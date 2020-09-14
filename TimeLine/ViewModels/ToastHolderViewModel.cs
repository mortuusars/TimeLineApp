using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using WpfScreenHelper;

namespace TimeLine
{
    public class ToastHolderViewModel
    {
        public event EventHandler LastToastClosed;                

        public ObservableCollection<ToastControlViewModel> ToastList { get; set; }

        public ToastHolderViewModel() {            
            ToastList = new ObservableCollection<ToastControlViewModel>();
        }


        public void ShowToast(string title, string message, Icons icon, bool IsAlarm = false) {
            
            ToastControlViewModel newToast = new ToastControlViewModel(title, message, icon);

            ToastList.Add(newToast);

            App.Manager.AddHistoryItem(new Models.HistoryItem(title, message, icon));

            // Play sound and don't close toast if it is "ALARM"
            if (IsAlarm == true) {
                App.SoundPlayer.Play();
            }
            // Close after delay if it is regular toast
            else {                
                var timerToClosing = new DispatcherTimer();            
                
                timerToClosing.Interval = TimeSpan.FromSeconds(App.ApplicationSettings.AppSettings.ToastNotificationDelayInSeconds);
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
            if (ToastList.Count == 0)
                LastToastClosed?.Invoke(this, EventArgs.Empty);
        }


    }
}
