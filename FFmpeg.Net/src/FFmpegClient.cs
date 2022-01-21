using FFmpeg.Net.Data;
using FFmpeg.Net.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        public async Task ConvertAsync(MediaFile media, VideoType destinationType)
        {
            try
            {
                using Process process = new();
                string ffmpegCommand = _commandCreator.Convert(media.Name, media.VideoType, destinationType);
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
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task SplitAsync(MediaFile media, int seconds)
        {
            try
            {
                using Process process = new();
                string ffmpegCommand = _commandCreator.Split(media.Name, media.VideoType, seconds);
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
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task MergeAsync(IEnumerable<MediaFile> mediaFiles, string destinationFileName, VideoType destinationType)
        {
            try
            {
                await using (StreamWriter writer = new("list.txt"))
                {
                    foreach (MediaFile mediaFile in mediaFiles)
                    {
                        await writer.WriteLineAsync($"file '{mediaFile.Name}.{mediaFile.VideoType.ToString().ToLower()}' ");
                    }
                }

                using Process process = new();
                string ffmpegCommand = _commandCreator.Merge(destinationFileName, destinationType);
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
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
