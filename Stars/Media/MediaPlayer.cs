namespace Stars.Media
{
    internal class Media_Player
    {
        private readonly MediaPlayer.MediaPlayer player;
        private const int volumeInterval = 5000;
        private int lastVolume;
        private bool isMute;
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
                if (value >= 0)
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
                player.Volume = (volumeInterval * _value / 100) - volumeInterval;
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
            Volume = 100;
        }

        public void Mute()
        {
            if (isMute)
            {
                Volume = lastVolume;
            }
            else
            {
                lastVolume = Volume;
                Volume = 0;
            }
            isMute = !isMute;
        }
    }
}