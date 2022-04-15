namespace Stars.Media
{
    class Media_Player
    {
        private readonly MediaPlayer.MediaPlayer player = new MediaPlayer.MediaPlayer();
        private const int maxVolume = 0;
        private const int minVolume = -6000;
        private int lastVolume = 0;
        private bool isMute = false;
        public string FileName { get; set; }
        public int Volume
        {
            get
            {
                return player.Volume;
            }
            set
            {
                if (value >= minVolume && value <= maxVolume)
                {
                    player.Volume = value;
                    lastVolume = value;
                }
            }
        }

        public Media_Player() : this(string.Empty) { }

        public Media_Player(string fileName)
        {
            player.PlayCount = int.MaxValue;
            FileName = fileName;
        }

        public void Open()
        {
            Open(FileName);
        }

        /// <summary>
        /// Открывает файл для воспроизведения
        /// </summary>
        public void Open(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && System.IO.File.Exists(fileName))
            {
                FileName = fileName;
                player.Open(fileName);
            }
        }

        /// <summary>
        /// Играет музон
        /// </summary>
        public void Play()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpPlaying)
            {
                player.Play();
            }
        }

        public void Stop()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpStopped)
            {
                player.Stop();
            }
        }

        public void Pause()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpPaused)
            {
                player.Pause();
            }
        }

        public void Mute()
        {
            if (isMute)
            {
                player.Volume = lastVolume;
            }
            else
            {
                player.Volume = minVolume;
            }
            isMute = !isMute;
        }
    }
}