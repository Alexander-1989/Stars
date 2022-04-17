namespace Stars.Media
{
    class Media_Player
    {
        private readonly MediaPlayer.MediaPlayer player = null;
        private const int maxVolume = 0;
        private const int minVolume = -6000;
        private const int volumeInterval = 6000;
        private int lastVolume = 0;
        private bool isMute = false;
        public string FileName { get; set; }
        public int Volume
        {
            get
            {
                return 100 * player.Volume / volumeInterval + 100;
            }
            set
            {
                int _value = value;
                if (_value >= 0)
                {
                    if (_value > 100)
                    {
                        _value = 100;
                    }
                }
                else
                {
                    _value = 0;
                }

                lastVolume = (volumeInterval * _value / 100) - volumeInterval;
                player.Volume = lastVolume;
            }
        }

        public Media_Player() : this("") { }

        public Media_Player(string fileName)
        {
            player = new MediaPlayer.MediaPlayer() { PlayCount = int.MaxValue };
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
            if (System.IO.File.Exists(fileName))
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

        public void SetMaxVolume()
        {
            isMute = false;
            lastVolume = maxVolume;
            player.Volume = lastVolume;
        }

        public void Mute()
        {
            player.Volume = isMute ? lastVolume : minVolume;
            isMute = !isMute;
        }
    }
}