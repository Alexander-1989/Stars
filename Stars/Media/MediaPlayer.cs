namespace Stars.Media
{
    internal class Media_Player
    {
        public delegate void PlayerEventHandler(object sender, PlayerEventArgs e);
        public event PlayerEventHandler Notify;
        private readonly MediaPlayer.MediaPlayer player;
        private const int volumeInterval = 4050;
        private const int maxVolume = 100;
        private const int minVolume = 0;
        private const int maxPlayCount = 2147483647;
        private int _lastVolume;
        private bool _isMute;
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
                if (_value > maxVolume) _value = maxVolume;
                else if (_value < minVolume) _value = minVolume;
                player.Volume = (volumeInterval * _value / 100) - volumeInterval;
                Notify?.Invoke(this, new PlayerEventArgs($"Volume: {_value}%"));
            }
        }

        public int PlayCount
        {
            get
            {
                return player.PlayCount;
            }
            set
            {
                player.PlayCount = value;
            }
        }

        public bool IsMute
        {
            get
            {
                return _isMute;
            }
        }

        public Media_Player() : this("") { }

        public Media_Player(string fileName)
        {
            player = new MediaPlayer.MediaPlayer();
            PlayCount = maxPlayCount;
            Volume = maxVolume;
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
                Notify?.Invoke(this, new PlayerEventArgs("Play"));
            }
        }

        public void Stop()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpStopped)
            {
                player.Stop();
                Notify?.Invoke(this, new PlayerEventArgs("Stop"));
            }
        }

        public void Pause()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpPaused)
            {
                player.Pause();
                Notify?.Invoke(this, new PlayerEventArgs("Pause"));
            }
        }

        public void SetMaxVolume()
        {
            _isMute = false;
            Volume = maxVolume;
        }

        public void Mute()
        {
            if (_isMute)
            {
                Volume = _lastVolume;
            }
            else
            {
                _lastVolume = Volume;
                Volume = minVolume;
            }
            _isMute = !_isMute;
        }
    }
}