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

        public async Task ConvertAsync(MediaFile media, VideoType destinationType, string destinationDirectory = "")
        {
            if (media is null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            var (name, videoType) = media;
            string ffmpegCommand = _commandCreator.Convert(_options.SourceFilePath, name, videoType, destinationType, destinationDirectory);
            await Run(ffmpegCommand);

            if (!_options.DeleteProcessedFile)
            {
                return;
            }

            string filePath = _commandCreator.GetFullPath(_options.SourceFilePath, name, videoType);
            File.Delete(filePath);
        }

        public async Task SplitAsync(MediaFile media, int seconds, string destinationDirectory = "")
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
                return;
            }

            string filePath = _commandCreator.GetFullPath(_options.SourceFilePath, name, videoType);
            File.Delete(filePath);
        }

        public async Task MergeAsync(ICollection<MediaFile> mediaFiles, string destinationFileName, VideoType destinationType, string destinationDirectory = "")
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
                    string filePath = _options.SourceFilePath.Length != 0
                        ? (Path.GetFullPath(_options.SourceFilePath) + @"\")
                        : null;
                    filePath += $"{name}.{videoType.ToString().ToLower()}";
                    await writer.WriteLineAsync($"file '{filePath}' ");
                }
            }

            string ffmpegCommand = _commandCreator.Merge(destinationFileName, destinationType, destinationDirectory);
            await Run(ffmpegCommand);

            File.Delete("list.txt");

            if (!_options.DeleteProcessedFile)
            {
                return;
            }

            foreach (var (name, videoType) in mediaFiles)
            {
                string filePath = _commandCreator.GetFullPath(_options.SourceFilePath, name, videoType);
                File.Delete(filePath);
            }
        }

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

        private static void CheckIsDirectoryValid(string directory)
        {
            if (!File.Exists(directory))
            {
                throw new Exception("FFmpeg file does not found");
            }
        }
    }
}
