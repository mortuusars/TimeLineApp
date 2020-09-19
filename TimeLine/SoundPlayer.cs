using System;
using System.Windows.Media;

namespace TimeLine
{
    public class SoundPlayer
    {
        string _soundFilePath;
        int _defaultVolume;

        public bool IsPlaying { get; set; }
        public bool IsMuted { get; set; }


        MediaPlayer player;
        double volumeBeforeMute;

        public SoundPlayer(string soundFilePath, int defaultVolume) {
            _soundFilePath = soundFilePath;
            _defaultVolume = defaultVolume;

            player = new MediaPlayer();
            SetVolume(_defaultVolume);
        }

        #region Public Methods        

        public void Play() {

            try {
                player.Open(new Uri(_soundFilePath, UriKind.Relative));
            }
            catch (Exception ex) {
                MortuusLogger.Logger.Log($"Error playing sound from file: {ex.Message}", MortuusLogger.LogLevel.ERROR);
                //TODO: Wrap sound methods
                throw;
            }

            player.MediaEnded += (s, e) => { OnPlaybackEnd(); };

            IsPlaying = true;
            player.Play();

        }

        public void Stop() {
            player.Stop();
            OnPlaybackEnd();
        }

        public void Mute() {
            volumeBeforeMute = player.Volume;
            SetVolume(0);
            IsMuted = true;
        }

        public void UnMute() {
            SetVolume(volumeBeforeMute);
            IsMuted = false;
        }
        

        public void SetVolume(int volume) {
            player.Volume = volume / 100.0f;
        }

        public void SetVolume(double volume) {
            player.Volume = volume;
        }

        #endregion

        private void OnPlaybackEnd() {
            IsPlaying = false; 
            player.Close();
        }






        /*
         
        const int FadeDelayMilliseconds = 30;

        private async void FadeOut() {
            if (IsMuted)
                return;

            int volumeFade = soundVolume;

            while (volumeFade > 0) {
                volumeFade -= 2;
                SetVolume(volumeFade);
                await Task.Delay(FadeDelayMilliseconds);
            }
        }

        */
    }
}
