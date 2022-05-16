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
        /// Is current file a stream
        /// </summary>
        public bool IsStream { get; init; }

        /// <summary>
        /// Stream Url
        /// </summary>
        public string StreamUrl { get; init; }

        /// <summary>
        /// File's name.
        /// </summary>
        public string FileName => IsStream ? Guid.NewGuid().ToString() : Path.GetFileName(FilePath);

        /// <summary>
        /// File's full path.
        /// </summary>
        public string FullPath => Path.GetFullPath(FilePath);

        public MediaFile(string filePath, bool isStream = false, string streamUrl = default)
        {
            FilePath = filePath;
            VideoType = isStream 
                ? (VideoType)Enum.Parse(typeof(VideoType),
                streamUrl.Split('.', StringSplitOptions.RemoveEmptyEntries)[^1].ToUpper())
                : (VideoType)Enum.Parse(typeof(VideoType),
                FileName.Split('.', StringSplitOptions.RemoveEmptyEntries)[1].ToUpper());
            IsStream = isStream;
            StreamUrl = streamUrl;
        }

        public MediaFile(string filePath, VideoType videoType, bool isStream = false, string streamUrl = default)
        {
            FilePath = filePath;
            VideoType = videoType;
            IsStream = isStream;
            StreamUrl = streamUrl;
        }

        public void Deconstruct(out string filePath, out VideoType videoType)
        {
            filePath = FilePath;
            videoType = VideoType;
        }
    }
}
