namespace Stars.Media
{
    class Media_Player
    {
        readonly MediaPlayer.MediaPlayer _player = new MediaPlayer.MediaPlayer();
        const int _maxVolume = 0;
        const int _minVolume = -6000;
        int _lastVolume = 0;
        bool _isMute = false;
        public string FileName { get; set; }
        public int Volume
        {
            get
            {
                return _player.Volume;
            }
            set
            {
                if (value >= _minVolume && value <= _maxVolume)
                {
                    _player.Volume = value;
                    _lastVolume = value;
                }
            }
        }

        public Media_Player() : this("") { }

        public Media_Player(string fileName)
        {
            _player.PlayCount = int.MaxValue;
            FileName = fileName;
        }

        public void Open()
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                Open(FileName);
            }
        }

        public void Open(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && System.IO.File.Exists(fileName))
            {
                FileName = fileName;
                _player.Open(fileName);
            }
        }

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
            if (_isMute)
            {
                _player.Volume = _lastVolume;
            }
            else
            {
                _player.Volume = _minVolume;
            }
            _isMute = !_isMute;
        }
    }
}