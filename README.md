# FFmpeg.Net

## Simple split example
```C#
FFmpegClientOptions options = new FFmpegClientOptions("ffmpeg.exe", true);
FFmpegClient client = new FFmpegClient(options);
string directory = await client.SplitAsync(new MediaFile("example.mp4"), 10, "splitedExample");
```
