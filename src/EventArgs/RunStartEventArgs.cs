namespace FFmpeg.Net.EventArgs
{
    public record RunStartEventArgs(string FFmpegCommand, FFmpegClientOptions Options);
}
