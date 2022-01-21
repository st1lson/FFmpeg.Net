using FFmpeg.Net.Data;
using FFmpeg.Net.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FFmpeg.Net
{
    public class FFmpegClient
    {
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
        public async Task<string> ConvertAsync(MediaFile media, VideoType destinationType, string destinationDirectory = "")
        {
            if (media is null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var (name, videoType) = media;
            string ffmpegCommand = _commandCreator.Convert(_options.SourceFilePath, name, videoType, destinationType, destinationDirectory);
            await Run(ffmpegCommand);

            string convertedFile = _commandCreator.GetFullPath(_options.SourceFilePath, name, videoType);

            if (!_options.DeleteProcessedFile)
            {
                return convertedFile;
            }

            string filePath = _commandCreator.GetFullPath(_options.SourceFilePath, name, videoType);
            File.Delete(filePath);

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
        public async Task<string> SplitAsync(MediaFile media, int seconds, string destinationDirectory = "")
        {
            if (media is null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var (name, videoType) = media;
            string ffmpegCommand = _commandCreator.Split(_options.SourceFilePath, name, videoType, seconds, destinationDirectory);
            await Run(ffmpegCommand);

            if (!_options.DeleteProcessedFile)
            {
                return destinationDirectory;
            }

            string filePath = _commandCreator.GetFullPath(_options.SourceFilePath, name, videoType);
            File.Delete(filePath);

            return destinationDirectory;
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
        public async Task<string> MergeAsync(ICollection<MediaFile> mediaFiles, string destinationFileName, VideoType destinationType, string destinationDirectory = "")
        {
            if (mediaFiles.Count == 0)
            {
                throw new NullReferenceException(nameof(mediaFiles));
            }

            await using (StreamWriter writer = new("list.txt"))
            {
                foreach (MediaFile mediaFile in mediaFiles)
                {
                    if (mediaFile is null)
                    {
                        throw new ArgumentNullException(nameof(mediaFile));
                    }

                    var (name, videoType) = mediaFile;
                    string filePath = _commandCreator.GetFullPath(_options.SourceFilePath, name, videoType);
                    await writer.WriteLineAsync($"file '{filePath}' ");
                }
            }

            string ffmpegCommand = _commandCreator.Merge(destinationFileName, destinationType, destinationDirectory);
            await Run(ffmpegCommand);

            File.Delete("list.txt");

            string mergedFile = _commandCreator.GetFullPath(_options.SourceFilePath, destinationFileName, destinationType);

            if (!_options.DeleteProcessedFile)
            {
                return mergedFile;
            }

            foreach (var (name, videoType) in mediaFiles)
            {
                string filePath = _commandCreator.GetFullPath(_options.SourceFilePath, name, videoType);
                File.Delete(filePath);
            }

            return mergedFile;
        }

        /// <summary>
        /// Runs the specified FFmpeg command.
        /// </summary>
        /// <param name="ffmpegCommand"></param>
        /// <returns></returns>
        public async Task Run(string ffmpegCommand)
        {
            using Process process = new();
            ProcessStartInfo startInfo = new()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = _options.FFmpegDirectory,
                Arguments = ffmpegCommand
            };
            process.StartInfo = startInfo;
            process.Start();

            await process.WaitForExitAsync();
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
                throw new Exception("FFmpeg file does not found");
            }
        }
    }
}
