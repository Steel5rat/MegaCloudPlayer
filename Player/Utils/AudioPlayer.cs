using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;

namespace Player.Utils
{
    public class AudioPlayer
    {
        private List<Stream> _playList;
        private int _playingPointer;
        private Stream _currentStream;
        private Mp3FileReader _currentWaveProvider;
        private readonly IWavePlayer _waveOutDevice = new WaveOut();

        public AudioPlayer()
        {
            _waveOutDevice.PlaybackStopped += _waveOutDevice_PlaybackStopped;
        }

        void _waveOutDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Forward();
        }

        public void Init(IList<Stream> streams)
        {
            _waveOutDevice.Stop();
            _playList.ForEach(e => e.Dispose());
            _playingPointer = 0;

            _playList = Shuffle(streams).ToList();
            LoadPlaying();
        }

        private void LoadPlaying()
        {
            if (_currentStream != null)
            {
                _currentStream.Dispose();
                _currentWaveProvider.Dispose();
            }

            byte[] bytes;
            using (var br = new BinaryReader(_playList[_playingPointer]))
            {
                bytes = br.ReadBytes((int)_playList[_playingPointer].Length);
            }
            _currentStream = new MemoryStream(bytes);
            _currentWaveProvider = new Mp3FileReader(_currentStream);
            _waveOutDevice.Init(_currentWaveProvider);
        }

        public void Back()
        {

        }

        public void Forward()
        {
            _playingPointer++;
            if (_playingPointer < _playList.Count)
            {
                LoadPlaying();
            }
        }

        public void Pause()
        {
            _waveOutDevice.Pause();
        }

        public void Play()
        {
            _waveOutDevice.Play();
        }

        private IEnumerable<Stream> Shuffle(IEnumerable<Stream> source)
        {
            var rnd = new Random();
            return source.OrderBy((item) => rnd.Next());
        }
    }
}