using FFmpeg.Net.Data;
using FFmpeg.Net.Enums;
using System;
using System.Diagnostics;
using System.IO;

namespace FFmpeg.Net
{
    public class FFmpegClient
    {
        private readonly string _ffmpegDirecotory;

        public FFmpegClient(string ffmpegDirecotry)
        {
            _ffmpegDirecotory = IsDirecotryValid(ffmpegDirecotry)
                ? ffmpegDirecotry
                : throw new Exception($"FFmpeg does not exists '{ffmpegDirecotry}'");
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
                    Arguments = $"ffmpeg -i {media.Name}.{media.VideoType.ToString().ToLower()} {media.Name}.{destinationType.ToString().ToLower()}"
                };
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception e)
            {
                throw new Exception(nameof(e));
            }
        }

        private static bool IsDirecotryValid(string directory)
        {
            if (!File.Exists(directory) || !directory.Contains("ffmpeg.exe"))
            {
                throw new Exception("FFmpeg file does not found");
            }

            return true;
        }
    }
}
