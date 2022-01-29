namespace FFmpeg.Net.EventArgs
{
    public record RunExceptionEventArgs(string FFmpegCommand, string Message, int ExitCode);
}
