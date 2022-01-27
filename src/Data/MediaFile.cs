using FFmpeg.Net.Enums;
using System;
using System.IO;

namespace FFmpeg.Net.Data
{
    public class MediaFile
    {
        /// <summary>
        /// File's path.
        /// </summary>
        public string FilePath { get; init; }

        /// <summary>
        /// File's video type.
        /// </summary>
        public VideoType VideoType { get; init; }

        /// <summary>
        /// File's name.
        /// </summary>
        public string FileName => Path.GetFileName(FilePath);

        /// <summary>
        /// File's full path.
        /// </summary>
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
