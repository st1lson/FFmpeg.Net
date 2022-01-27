using FFmpeg.Net.Data;
using FFmpeg.Net.Enums;
using FFmpeg.Net.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFmpeg.Net
{
    public class FFmpegClient
    {
        /// <summary>
        /// Fires when a process has started.
        /// </summary>
        public event Func<RunStartEventArgs, Task> OnRunStarted;

        /// <summary>
        /// Fires when a process has ended.
        /// </summary>
        public event Func<RunEndedEventArgs, Task> OnRunEnded;

        /// <summary>
        /// Fires when a process has thrown an exception.
        /// </summary>
        public event Func<RunExceptionEventArgs, Task> OnRunException;

        private readonly FFmpegClientOptions _options;
        private readonly CommandCreator _commandCreator;

        public FFmpegClient(FFmpegClientOptions options)
        {
            CheckIsDirectoryValid(options.FFmpegDirectory);
            _options = options;
            _commandCreator = new CommandCreator();
        }

        /// <summary>
        /// Converts a media file into the selected video type.
        /// </summary>
        /// <param name="media"></param>
        /// <param name="destinationType"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="media"/> is null.</exception>
        public async Task<string> ConvertAsync(MediaFile media, VideoType destinationType, string destinationDirectory)
        {
            if (media is null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(Path.GetFullPath(destinationDirectory)))
            {
                Directory.CreateDirectory(Path.GetFullPath(destinationDirectory));
            }

            string ffmpegCommand = _commandCreator.Convert(media, destinationType, destinationDirectory);
            await RunAsync(ffmpegCommand).ConfigureAwait(false);

            string convertedFile = Path.GetFullPath(_commandCreator.GetFullPath(
                destinationDirectory,
                Path.GetFileNameWithoutExtension(media.FilePath),
                destinationType)
            );

            if (!_options.DeleteProcessedFile)
            {
                return convertedFile;
            }

            File.Delete(media.FullPath);

            return convertedFile;
        }

        /// <summary>
        /// Splits a media file into files with the selected duration.
        /// </summary>
        /// <param name="media"></param>
        /// <param name="seconds"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="media"/> is null.</exception>
        public async Task<string> SplitAsync(MediaFile media, int seconds, string destinationDirectory)
        {
            if (media is null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(Path.GetFullPath(destinationDirectory)))
            {
                Directory.CreateDirectory(Path.GetFullPath(destinationDirectory));
            }

            string ffmpegCommand = _commandCreator.Split(media, seconds, destinationDirectory);
            await RunAsync(ffmpegCommand).ConfigureAwait(false);

            string fullPath = string.IsNullOrEmpty(destinationDirectory)
                ? destinationDirectory
                : Path.GetFullPath(destinationDirectory);

            if (!_options.DeleteProcessedFile)
            {
                return fullPath;
            }

            File.Delete(media.FullPath);

            return fullPath;
        }

        /// <summary>
        /// Merges a collection of media files into one file.
        /// </summary>
        /// <param name="mediaFiles"></param>
        /// <param name="destinationFileName"></param>
        /// <param name="destinationType"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Throws when <paramref name="mediaFiles"/> is an empty collection.</exception>
        /// <exception cref="ArgumentNullException">Throws when the <paramref name="mediaFiles"/> element is null.</exception>
        public async Task<string> MergeAsync(ICollection<MediaFile> mediaFiles,
            string destinationFileName,
            VideoType destinationType,
            string destinationDirectory)
        {
            if (mediaFiles.Count == 0)
            {
                throw new NullReferenceException(nameof(mediaFiles));
            }

            if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(Path.GetFullPath(destinationDirectory)))
            {
                Directory.CreateDirectory(Path.GetFullPath(destinationDirectory));
            }

            await using (StreamWriter writer = new("list.txt"))
            {
                foreach (MediaFile mediaFile in mediaFiles)
                {
                    if (mediaFile is null)
                    {
                        throw new ArgumentNullException(nameof(mediaFile));
                    }

                    await writer.WriteLineAsync($"file '{mediaFile.FullPath}' ").ConfigureAwait(false);
                }
            }

            string ffmpegCommand = _commandCreator.Merge(destinationFileName, destinationType, destinationDirectory);
            await RunAsync(ffmpegCommand).ConfigureAwait(false);

            File.Delete("list.txt");

            string mergedFile = _commandCreator.GetFullPath(destinationDirectory, destinationFileName, destinationType);

            if (!_options.DeleteProcessedFile)
            {
                return mergedFile;
            }

            foreach (MediaFile media in mediaFiles)
            {
                File.Delete(media.FullPath);
            }

            return mergedFile;
        }

        /// <summary>
        /// Runs the specified FFmpeg command.
        /// </summary>
        /// <param name="ffmpegCommand"></param>
        /// <returns></returns>
        public async Task RunAsync(string ffmpegCommand)
        {
            using Process process = new();
            ProcessStartInfo startInfo = new()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = _options.FFmpegDirectory,
                RedirectStandardError = true,
                Arguments = ffmpegCommand
            };
            process.StartInfo = startInfo;
            process.Start();

            if (OnRunStarted is not null)
            {
                await OnRunStarted.Invoke(new RunStartEventArgs(_options, ffmpegCommand)).ConfigureAwait(false);
            }

            string lastLine = null;
            StringBuilder runMessage = new();
            while (!process.StandardError.EndOfStream)
            {
                string line = await process.StandardError.ReadLineAsync().ConfigureAwait(false);
                runMessage.AppendLine(line);
                lastLine = line;
            }

            await process.WaitForExitAsync().ConfigureAwait(false);

            if ((process.ExitCode != 0 || !ContainsError(lastLine)) && OnRunException is not null)
            {
                await OnRunException!.Invoke(new RunExceptionEventArgs(ffmpegCommand, lastLine, process.ExitCode))
                    .ConfigureAwait(false);
            }
            else if (OnRunEnded is not null)
            {
                await OnRunEnded!.Invoke(new RunEndedEventArgs(ffmpegCommand, runMessage.ToString())).ConfigureAwait(false);
            }
        }

        private static bool ContainsError(string errors)
        {
            return errors.Contains("muxing overhead");
        }

        /// <summary>
        /// Checks if the FFmpeg directory is valid.
        /// </summary>
        /// <param name="directory"></param>
        /// <exception cref="Exception">Throws when the FFmpeg file is not found</exception>
        private static void CheckIsDirectoryValid(string directory)
        {
            if (!File.Exists(directory))
            {
                throw new FileNotFoundException(directory);
            }

            if (!Regex.IsMatch(Path.GetFileName(directory), @"ffmpeg(\.exe)?$"))
            {
                throw new Exception("FFmpeg file does not found");
            }
        }
    }
}
