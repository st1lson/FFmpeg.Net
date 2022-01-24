namespace FFmpeg.Net.EventArgs
{
    public readonly struct RunStartEventArgs
    {
        public string FFmpegCommand { get; }

        public FFmpegClientOptions Options { get; }

        public RunStartEventArgs(FFmpegClientOptions options, string fFmpegCommand)
        {
            Options = options;
            FFmpegCommand = fFmpegCommand;
        }
    }
}
