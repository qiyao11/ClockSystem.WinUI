using System.IO;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace ClockSystem.WinUI.Services
{
    public class AudioService
    {
        private readonly MediaPlayer _player = new MediaPlayer();

        public void PlayAudio(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                var source = MediaSource.CreateFromStorageFile(
                    Windows.Storage.StorageFile.GetFileFromPathAsync(filePath).AsTask().Result);
                _player.Source = source;
                _player.Play();
                
                while (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    System.Threading.Tasks.Task.Delay(100).Wait();
                }
            }
            catch
            {
            }
        }
    }
}
