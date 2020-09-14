using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TimeLine
{
    public class SoundPlayer
    {
        AppSettings settings = App.ApplicationSettings.AppSettings;

        string soundPath;
        int defaultVolume;

        const int FadeDelayMilliseconds = 30;


        public bool IsPlaying { get; set; }
        public bool IsMuted { get; set; }


        MediaPlayer player;
        double volumeBeforeMute;

        public SoundPlayer() {
            soundPath = settings.SoundFilePath;
            defaultVolume = settings.SoundVolume;

            player = new MediaPlayer();
            SetVolume(defaultVolume);
        }

        #region Public Methods        

        public void Play() {

            try {
                player.Open(new Uri(soundPath, UriKind.Relative));
            }
            catch (Exception ex) {
                MortuusLogger.Logger.Log($"Error playing sound from file: {ex.Message}", MortuusLogger.LogLevel.ERROR);
                return;
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
