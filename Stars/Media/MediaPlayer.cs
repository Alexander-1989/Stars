namespace Stars.Media
{
    internal class Media_Player
    {
        public delegate void PlayerStateHandler(object sender, PlayerEventArgs e);
        public event PlayerStateHandler State;
        private readonly MediaPlayer.MediaPlayer player;
        private const int volumeInterval = 5000;
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
                if (_value > 100) _value = 100;
                else if (_value < 0) _value = 0;
                player.Volume = (volumeInterval * _value / 100) - volumeInterval;
                State?.Invoke(this, new PlayerEventArgs($"Volume: {_value}%"));
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
            PlayCount = int.MaxValue;
            Volume = 100;
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
                State?.Invoke(this, new PlayerEventArgs("Play"));
            }
        }

        public void Stop()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpStopped)
            {
                player.Stop();
                State?.Invoke(this, new PlayerEventArgs("Stop"));
            }
        }

        public void Pause()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpPaused)
            {
                player.Pause();
                State?.Invoke(this, new PlayerEventArgs("Pause"));
            }
        }

        public void SetMaxVolume()
        {
            _isMute = false;
            Volume = 100;
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
                Volume = 0;
                State?.Invoke(this, new PlayerEventArgs("Mute"));
            }
            _isMute = !_isMute;
        }
    }

    class PlayerEventArgs
    {
        public string Message { get; }

        public PlayerEventArgs(string message)
        {
            Message = message;
        }
    }
}