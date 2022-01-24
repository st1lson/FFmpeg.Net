namespace FFmpeg.Net.EventArgs
{
    public readonly struct RunExceptionEventArgs
    {
        public string FFmpegCommand { get; }

        public int ExitCode { get; }

        public RunExceptionEventArgs(string fFmpegCommand, int exitCode)
        {
            FFmpegCommand = fFmpegCommand;
            ExitCode = exitCode;
        }
    }
}
