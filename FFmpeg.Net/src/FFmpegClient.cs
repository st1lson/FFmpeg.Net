using FFmpeg.Net.Data;
using FFmpeg.Net.Enums;
using System;
using System.Diagnostics;
using System.IO;

namespace FFmpeg.Net
{
    public class FFmpegClient
    {
        private readonly FFmpegClientOptions _options;
        private readonly CommandCreator _commandCreator;

        public FFmpegClient(FFmpegClientOptions options)
        {
            _options = options;
        }

        public void Convert(MediaFile media, VideoType destinationType)
        {
            try
            {
                Process process = new();
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = _commandCreator.Convert(media.Name, media.VideoType, destinationType)
                };
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception(nameof(e));
            }
        }

        public void Split(MediaFile media, int seconds)
        {
            try
            {
                Process process = new();
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = _commandCreator.Split(media.Name, media.VideoType, seconds)
                };
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception(nameof(e));
            }
        }

        private static void CheckDirecotryValid(string directory)
        {
            if (!File.Exists(directory) || !directory.Contains("ffmpeg.exe"))
            {
                throw new Exception("FFmpeg file does not found");
            }
        }
    }
}
