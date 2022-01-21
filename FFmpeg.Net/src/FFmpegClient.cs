﻿using FFmpeg.Net.Data;
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
            try
            {
                var (name, videoType) = media;
                using Process process = new();
                string ffmpegCommand = _commandCreator.Convert(_options.SourceFilePath, name, videoType, destinationType, destinationDirectory);
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = _options.FFmpegDirectory,
                    Arguments = ffmpegCommand
                };
                process.StartInfo = startInfo;
                process.Start();

                await process.WaitForExitAsync();

                if (!_options.DeleteProcessedFile)
                {
                    return;
                }

                string filePath = _options.SourceFilePath.Length != 0
                    ? (Path.GetFullPath(_options.SourceFilePath) + @"\")
                    : null;
                filePath += $"{name}.{videoType.ToString().ToLower()}";
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task SplitAsync(MediaFile media, int seconds, string destinationDirectory = "")
        {
            try
            {
                var (name, videoType) = media;
                using Process process = new();
                string ffmpegCommand = _commandCreator.Split(_options.SourceFilePath, name, videoType, seconds, destinationDirectory);
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = _options.FFmpegDirectory,
                    Arguments = ffmpegCommand
                };
                process.StartInfo = startInfo;
                process.Start();

                await process.WaitForExitAsync();

                if (!_options.DeleteProcessedFile)
                {
                    return;
                }

                string filePath = _options.SourceFilePath.Length != 0
                    ? (Path.GetFullPath(_options.SourceFilePath) + @"\")
                    : null;
                filePath += $"{name}.{videoType.ToString().ToLower()}";
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task MergeAsync(IEnumerable<MediaFile> mediaFiles, string destinationFileName, VideoType destinationType, string destinationDirectory = "")
        {
            try
            {
                await using (StreamWriter writer = new("list.txt"))
                {
                    foreach (var (name, videoType) in mediaFiles)
                    {
                        string filePath = _options.SourceFilePath.Length != 0
                            ? (Path.GetFullPath(_options.SourceFilePath) + @"\")
                            : null;
                        filePath += $"{name}.{videoType.ToString().ToLower()}";
                        await writer.WriteLineAsync($"file '{filePath}' ");
                    }
                }

                using Process process = new();
                string ffmpegCommand = _commandCreator.Merge(destinationFileName, destinationType, destinationDirectory);
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = _options.FFmpegDirectory,
                    Arguments = ffmpegCommand
                };
                process.StartInfo = startInfo;
                process.Start();

                await process.WaitForExitAsync();

                File.Delete("list.txt");

                if (!_options.DeleteProcessedFile)
                {
                    return;
                }

                foreach (var (name, videoType) in mediaFiles)
                {
                    string filePath = _options.SourceFilePath.Length != 0
                        ? (Path.GetFullPath(_options.SourceFilePath) + @"\")
                        : null;
                    filePath += $"{name}.{videoType.ToString().ToLower()}";
                    File.Delete(filePath);
                }
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
