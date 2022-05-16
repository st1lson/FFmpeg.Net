using FFmpeg.Net.Data;
using FFmpeg.Net.Enums;
using System.IO;
using System.Text;

namespace FFmpeg.Net
{
    internal sealed class CommandCreator
    {
        public string Convert(MediaFile mediaFile, VideoType destinationType, string destinationDirectory)
        {
            StringBuilder builder = new();
            builder.Append($@"-i {(mediaFile.IsStream ? mediaFile.StreamUrl : mediaFile.FullPath)} ");
            builder.Append(
                $"{GetFullPath(destinationDirectory, Path.GetFileNameWithoutExtension(mediaFile.FileName), destinationType)}");

            return builder.ToString();
        }

        public string Split(MediaFile mediaFile, int seconds, string destinationDirectory)
        {
            StringBuilder builder = new();
            builder.Append($@"-i {(mediaFile.IsStream ? mediaFile.StreamUrl : mediaFile.FullPath)} ");
            builder.Append($"-c copy -map 0 -segment_time {seconds} -f segment -reset_timestamps 1 ");
            builder.Append(
                $"{GetFullPath(destinationDirectory, Path.GetFileNameWithoutExtension(mediaFile.FileName), mediaFile.VideoType, true)}");

            return builder.ToString();
        }

        public string Merge(string destinationFileName, VideoType destinationType, string destinationDirectory)
        {
            StringBuilder builder = new();
            builder.Append("-safe 0 -f concat -i list.txt -c copy ");
            builder.Append($"{GetFullPath(destinationDirectory, destinationFileName, destinationType)}");

            return builder.ToString();
        }

        internal string GetFullPath(string sourceFile, string fileName, VideoType videoType, bool isSplit = false)
        {
            StringBuilder builder = new();
            builder.Append($"{(sourceFile.Length != 0 ? (Path.GetFullPath(sourceFile) + @"\") : null)}");
            builder.Append($"{fileName}{(isSplit ? "%03d." : '.')}{videoType.ToString().ToLower()}");

            return builder.ToString();
        }
    }
}
