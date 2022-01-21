using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FFmpeg.Net;
using FFmpeg.Net.Data;
using FFmpeg.Net.Enums;

namespace Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FFmpegClientOptions options = new(@"FFmpegLib\bin\ffmpeg.exe", "", false);
            FFmpegClient client = new(options);
            List<MediaFile> mediaFiles = new()
            {
                new MediaFile("videoplayback000", VideoType.MP4),
                new MediaFile("videoplayback001", VideoType.MP4)
            };
            await client.MergeAsync(mediaFiles, "merged", VideoType.MP4);
        }
    }
}
