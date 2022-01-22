using FFmpeg.Net.Enums;
using System;
using System.IO;

namespace FFmpeg.Net.Data
{
    public class MediaFile
    {
        public string FilePath { get; init; }

        public VideoType VideoType { get; init; }

        public string FileName => Path.GetFileName(FilePath);

        public string FullPath => Path.GetFullPath(FilePath);

        public MediaFile(string filePath)
        {
            FilePath = filePath;
            VideoType = (VideoType)Enum.Parse(typeof(VideoType),
                FileName.Split('.', StringSplitOptions.RemoveEmptyEntries)[1].ToUpper());
        }

        public MediaFile(string filePath, VideoType videoType)
        {
            FilePath = filePath;
            VideoType = videoType;
        }

        public void Deconstruct(out string filePath, out VideoType videoType)
        {
            filePath = FilePath;
            videoType = VideoType;
        }
    }
}
