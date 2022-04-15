namespace Stars.Media
{
    class Media_Player
    {
        private readonly MediaPlayer.MediaPlayer _player = new MediaPlayer.MediaPlayer();
        private const int maxVolume = 0;
        private const int minVolume = -6000;
        private int lastVolume = 0;
        private bool isMute = false;
        public string FileName { get; set; }
        public int Volume
        {
            get { return _player.Volume; }
            set
            {
                if (value >= minVolume && value <= maxVolume)
                {
                    _player.Volume = value;
                    lastVolume = value;
                }
            }
        }

        public Media_Player() : this(string.Empty) { }

        public Media_Player(string fileName)
        {
            _player.PlayCount = int.MaxValue;
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
                this.FileName = fileName;
                _player.Open(fileName);
            }
        }

        /// <summary>
        /// Играет музон
        /// </summary>
        public void Play()
        {
            if (_player.PlayState != MediaPlayer.MPPlayStateConstants.mpPlaying)
            {
                _player.Play();
            }
        }

        public void Stop()
        {
            if (_player.PlayState != MediaPlayer.MPPlayStateConstants.mpStopped)
            {
                _player.Stop();
            }
        }

        public void Pause()
        {
            if (_player.PlayState != MediaPlayer.MPPlayStateConstants.mpPaused)
            {
                _player.Pause();
            }
        }

        public void Mute()
        {
            if (isMute)
            {
                _player.Volume = lastVolume;
            }
            else
            {
                _player.Volume = minVolume;
            }
            isMute = !isMute;
        }
    }
}