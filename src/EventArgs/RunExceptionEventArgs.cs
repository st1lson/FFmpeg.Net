namespace FFmpeg.Net.EventArgs
{
    public readonly struct RunExceptionEventArgs
    {
        public string FFmpegCommand { get; }

        public string Message { get; }

        public int ExitCode { get; }

        public RunExceptionEventArgs(string fFmpegCommand, string message, int exitCode)
        {
            FFmpegCommand = fFmpegCommand;
            Message = message;
            ExitCode = exitCode;
        }
    }
}
