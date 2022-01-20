namespace FFmpeg.Net
{
    public record FFmpegClientOptions(string FFmpegDirectory, string FilePath, bool DeleteProcessedFile);
}
