using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFmpeg.Net.Data;
using FFmpeg.Net.Enums;

namespace FFmpeg.Net
{
    internal sealed class CommandCreator
    {
        public string Convert(string fileName, VideoType fileVideoType, VideoType destinationType)
        {
            return $@"-i {fileName}.{fileVideoType.ToString().ToLower()} {fileName}.{destinationType.ToString().ToLower()}";
        }

        public string Split(string fileName, VideoType fileVideoType, int seconds)
        {
            return $@"-i {fileName}.{fileVideoType.ToString().ToLower()} -c copy -map 0 -segment_time 00:00:{seconds} -f segment -reset_timestamps 1 {fileName}%03d.{fileVideoType.ToString().ToLower()}";
        }

        public string Merge(string destinationFileName, VideoType destinationType)
        {
            return $@"-safe 0 -f concat -i list.txt -c copy {destinationFileName}.{destinationType.ToString().ToLower()}";
        }
    }
}
