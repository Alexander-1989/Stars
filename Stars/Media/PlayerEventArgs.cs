namespace Stars.Media
{
    internal class PlayerEventArgs
    {
        public string Message { get; }

        public PlayerEventArgs(string message)
        {
            Message = message;
        }
    }
}