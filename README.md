# FFmpeg.Net

## Quick start
- Download FFmpeg.Net package from NuGet
- Create options and add it to the FFmpeg client
```C#
            FFmpegClientOptions options = new FFmpegClientOptions("ffmpeg.exe", true);
            FFmpegClient client = new FFmpegClient(options);
```
- Convert a media file into another file type
```C#
            string filePath = await client.ConvertAsync(new MediaFile("example.mp4"), VideoType.AVI,
                Environment.CurrentDirectory);
```
- Split a media file into a collection of media files
```C#
            string directory = await client.SplitAsync(new MediaFile("example.mp4"), 10, "splitedExample");
```
- Merge a collection of media files to a single file.

```C#
            List<MediaFile> mediaFiles = new()
            {
                new MediaFile("splitedPart000.mp4"),
                new MediaFile("splitedPart001.mp4")
            };
            string filePath = await client.MergeAsync(mediaFiles, "mergedPart", VideoType.MP4, "mergedFiles");
```
