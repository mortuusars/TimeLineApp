using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using WpfScreenHelper;

namespace TimeLine
{
    public class ToastHolderViewModel
    {

        public double Width { get; set; } = 500;
        public double Height { get; set; } = 640;

        public double Left { get; set; }
        public double Top { get; set; }

        public ObservableCollection<ToastControlViewModel> ToastList { get; set; }


        public ToastHolderViewModel() {
            Left = Screen.PrimaryScreen.Bounds.Right - Width;
            Top = Screen.PrimaryScreen.Bounds.Bottom - Height * 1.05;

            ToastList = new ObservableCollection<ToastControlViewModel>();
        }


        public void ShowToast(string title, string message, Icons icon, bool IsAlarm = false) {
            
            ToastControlViewModel newToast = new ToastControlViewModel(title, message, icon);

            ToastList.Add(newToast);

            // Play sound and don't close toast if it is "ALARM"
            if (IsAlarm == true) {
                GetService.SoundPlayer.Play();
            }
            // Close after delay if it is regular toast
            else {
                
                var timerToClosing = new DispatcherTimer();
                //TODO: Easier changing of parameters like this:   <move them to some kind of settings>
                timerToClosing.Interval = TimeSpan.FromSeconds(6);
                timerToClosing.Tick += (sender, e) =>
                {
                    CloseToast(newToast);
                    timerToClosing.Stop();
                };

                timerToClosing.Start();
            }
        }

        
        public void CloseToast(ToastControlViewModel toast) {
            
            // Trigger fading animation on toast control
            toast.IsRemoving = true;

            // Create timer to remove toast from list after animation is done playing
            var timer = new DispatcherTimer();            
            timer.Interval = toast.FadeAnimationDuration.TimeSpan + toast.SlideAnimationDuration;
            timer.Tick += (sender, e) =>
            {
                RemoveToastFromList(toast);
                timer.Stop();
            };
            
            timer.Start();
        }

        private void RemoveToastFromList(ToastControlViewModel toast) {
            ToastList.Remove(toast);
        }


    }
}
