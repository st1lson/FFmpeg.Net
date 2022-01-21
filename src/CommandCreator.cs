using FFmpeg.Net.Enums;
using System.IO;
using System.Text;

namespace FFmpeg.Net
{
    internal sealed class CommandCreator
    {
        public string Convert(string sourceFilePath, string fileName, VideoType fileVideoType, VideoType destinationType, string destinationDirectory)
        {
            StringBuilder builder = new();
            builder.Append($@"-i {(sourceFilePath.Length != 0 ? (Path.GetFullPath(sourceFilePath) + "/") : null)}");
            builder.Append($"{fileName}.{fileVideoType.ToString().ToLower()} ");
            builder.Append(
                $"{(destinationDirectory.Length != 0 ? (Path.GetFullPath(destinationDirectory) + "/") : null)}");
            builder.Append($"{fileName}.{destinationType.ToString().ToLower()}");

            return builder.ToString();
        }

        public string Split(string sourceFilePath, string fileName, VideoType fileVideoType, int seconds, string destinationDirectory)
        {
            StringBuilder builder = new();
            builder.Append($@"-i {(sourceFilePath.Length != 0 ? (Path.GetFullPath(sourceFilePath) + "/") : null)}");
            builder.Append($"{fileName}.{fileVideoType.ToString().ToLower()} ");
            builder.Append($"-c copy -map 0 -segment_time 00:00:{seconds} -f segment -reset_timestamps 1 ");
            builder.Append(destinationDirectory.Length != 0 ? (Path.GetFullPath(destinationDirectory) + "/") : null);
            builder.Append($"{fileName}%03d.{fileVideoType.ToString().ToLower()}");

            return builder.ToString();
        }

        public string Merge(string destinationFileName, VideoType destinationType, string destinationDirectory)
        {
            StringBuilder builder = new();
            builder.Append($@"-safe 0 -f concat -i list.txt -c copy ");
            builder.Append(destinationDirectory.Length != 0 ? (Path.GetFullPath(destinationDirectory) + "/") : null);
            builder.Append($"{destinationFileName}.{destinationType.ToString().ToLower()}");

            return builder.ToString();
        }

        internal string GetFullPath(string sourceFile, string fileName, VideoType videoType)
        {
            StringBuilder builder = new();
            builder.Append($"{(sourceFile.Length != 0 ? (Path.GetFullPath(sourceFile) + @"\") : null)}");
            builder.Append($"{fileName}.{videoType.ToString().ToLower()}");

            return builder.ToString();
        }
    }
}
