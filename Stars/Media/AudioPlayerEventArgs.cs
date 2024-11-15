namespace Stars.Media
{
    internal class AudioPlayerEventArgs : System.EventArgs
    {
        public string Message { get; } = null;

        public AudioPlayerEventArgs(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
        }
    }
}