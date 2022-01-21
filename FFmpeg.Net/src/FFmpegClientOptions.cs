namespace FFmpeg.Net
{
    public record FFmpegClientOptions(string FFmpegDirectory, string SourceFilePath, bool DeleteProcessedFile);
}
