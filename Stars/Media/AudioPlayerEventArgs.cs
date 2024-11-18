namespace Stars.Media
{
    internal class AudioPlayerEventArgs : System.EventArgs
    {
        public string Message { get; } = null;

        public AudioPlayerEventArgs(string message)
        {
            Message = message ?? string.Empty;
        }
    }
}