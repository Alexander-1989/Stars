using System.IO;

namespace Stars.Media
{
    class Media_Player
    {
        readonly MediaPlayer.MediaPlayer _mPlayer = new MediaPlayer.MediaPlayer();
        const int _minVolume = -6000;
        const int _maxVolume = 0;
        int _lastVolume = 0;
        bool _isMute = false;
        public string FileName { get; set; }
        public int Volume
        {
            get
            {
                return _mPlayer.Volume;
            }
            set
            {
                if (value >= _minVolume && value <= _maxVolume)
                {
                    _mPlayer.Volume = value;
                    _lastVolume = value;
                }
            }
        }

        public Media_Player() : this("") { }

        public Media_Player(string fileName)
        {
            _mPlayer.PlayCount = int.MaxValue;
            FileName = fileName;
        }

        public void Open()
        {
            Open(FileName);
        }

        public void Open(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                FileName = fileName;
                _mPlayer.Open(fileName);
            }
        }

        public void Play()
        {
            if (_mPlayer.PlayState != MediaPlayer.MPPlayStateConstants.mpPlaying)
            {
                _mPlayer.Play();
            }
        }

        public void Stop()
        {
            if (_mPlayer.PlayState != MediaPlayer.MPPlayStateConstants.mpStopped)
            {
                _mPlayer.Stop();
            }
        }

        public void Pause()
        {
            if (_mPlayer.PlayState != MediaPlayer.MPPlayStateConstants.mpPaused)
            {
                _mPlayer.Pause();
            }
        }

        public void Mute()
        {
            if (_isMute)
            {
                _mPlayer.Volume = _lastVolume;
            }
            else
            {
                _mPlayer.Volume = _minVolume;
            }
            _isMute = !_isMute;
        }
    }
}