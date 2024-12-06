namespace Stars.Media
{
    internal class AudioPlayer
    {
        public delegate void PlayerEventHandler(object sender, AudioPlayerEventArgs e);
        public event PlayerEventHandler Notify;
        public bool IsMute { get; private set; }
        private readonly MediaPlayer.MediaPlayer player;
        private const int volumeInterval = 4050;
        private const int maxVolume = 100;
        private const int minVolume = 0;
        private int lastVolume;
        public string FileName { get; set; }
        public int Volume
        {
            get
            {
                return (maxVolume * player.Volume / volumeInterval) + maxVolume;
            }
            set
            {
                int volume = value > maxVolume ? maxVolume : value < minVolume ? minVolume : value;
                player.Volume = (volumeInterval * volume / maxVolume) - volumeInterval;
                Notify?.Invoke(this, new AudioPlayerEventArgs($"Volume: {volume}%"));
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

        public AudioPlayer() : this("") { }

        public AudioPlayer(string fileName)
        {
            player = new MediaPlayer.MediaPlayer();
            PlayCount = 0;
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
                Notify?.Invoke(this, new AudioPlayerEventArgs("Play"));
            }
        }

        public void Stop()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpStopped)
            {
                player.Stop();
                Notify?.Invoke(this, new AudioPlayerEventArgs("Stop"));
            }
        }

        public void Pause()
        {
            if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpPaused)
            {
                player.Pause();
                Notify?.Invoke(this, new AudioPlayerEventArgs("Pause"));
            }
        }

        public void SetMaxVolume()
        {
            IsMute = false;
            Volume = maxVolume;
        }

        public void Mute()
        {
            if (IsMute)
            {
                Volume = lastVolume;
            }
            else
            {
                lastVolume = Volume;
                Volume = minVolume;
            }

            IsMute = !IsMute;
        }
    }
}