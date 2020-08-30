using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TimeLine
{
    public static class SoundPlayer
    {
        //TODO: Properties
        static readonly string soundPath = "Resources/Sounds/subtle.wav";
        static readonly int soundVolume = 50;

        const int FadeDelayMilliseconds = 30;

        static MediaPlayer player = new MediaPlayer();

        static double volumeBeforeMute;
        static bool isPlaying;
        static bool isMuted;

        public static void Play() {
            player.MediaEnded += (s, e) => { isPlaying = false; };

            try {
                player.Open(new Uri(soundPath, UriKind.Relative));
            }
            catch (Exception ex) {
                MortuusLogger.Logger.Log($"Error playing sound from file: {ex.Message}", MortuusLogger.LogLevel.ERROR);
                return;
            }

            SetVolume(soundVolume);

            isPlaying = true;
            player.Play();

        }

        public static void Stop() {
            player.Stop();
            isPlaying = false;
        }

        /*
        public static async void Play() {
            player.MediaEnded += Player_MediaEnded;

            if (isPlaying == true)
                return;

            player.Open(new Uri(soundPath, UriKind.Relative));
            isPlaying = true;

            player.Stop();

            SetVolume(0);
            player.Play();

            FadeIn();
            await Task.Delay(FadeDelayMilliseconds * soundVolume);
        }

        private static void Player_MediaEnded(object sender, EventArgs e) {
            isPlaying = false;
        }
        



        public static async void Stop() {
            FadeOut();
            await Task.Delay(FadeDelayMilliseconds * soundVolume);
            player.Stop();
            isPlaying = false;
        }

        */

        /// <summary>
        /// Returns True if sound is playing and can be paused.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlaying() {
            return isPlaying;
        }

        /// <summary>
        /// Change sound volume.  
        /// </summary>
        /// <param name="volume">Number from 0 to 100.</param>
        public static void SetVolume(int volume) {
            player.Volume = volume / 100.0f;
        }

        public static void ToggleMute() {
            if (isMuted) {
                player.Volume = volumeBeforeMute;
                isMuted = false;
            }
            else {
                volumeBeforeMute = player.Volume;
                player.Volume = 0;
                isMuted = true;
            }

        }

        public static double GetVolume() {
            return player.Volume;
        }


        private static async void FadeIn() {
            if (isMuted)
                return;

            int volumeFadeIn = 0;
            while (volumeFadeIn < soundVolume) {
                volumeFadeIn += 1;
                SetVolume(volumeFadeIn);
                await Task.Delay(FadeDelayMilliseconds);
            }
        }

        private static async void FadeOut() {
            if (isMuted)
                return;

            int volumeFade = soundVolume;

            while (volumeFade > 0) {
                volumeFade -= 2;
                SetVolume(volumeFade);
                await Task.Delay(FadeDelayMilliseconds);
            }
        }
    }
}
