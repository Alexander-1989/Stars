namespace Stars.Media
{
    internal class PlayerEventArgs : System.EventArgs
    {
        public string Message { get; }

        public PlayerEventArgs(string message)
        {
            Message = message;
        }
    }
}