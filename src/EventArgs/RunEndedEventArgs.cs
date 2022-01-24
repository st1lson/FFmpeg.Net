namespace FFmpeg.Net.EventArgs
{
    public readonly struct RunEndedEventArgs
    {
        public string FFmpegCommand { get; }

        public string RunMessage { get; }

        public RunEndedEventArgs(string fFmpegCommand, string runMessage)
        {
            FFmpegCommand = fFmpegCommand;
            RunMessage = runMessage;
        }
    }
}
