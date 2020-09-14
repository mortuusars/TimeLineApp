using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TimeLine.ViewModels;

namespace TimeLine
{
    public class ToastControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }
        public SolidColorBrush IconTintColor { get; set; }

        //TODO: Postpone button?
        public bool PostponeButtonIsVisible { get; set; } = false;

        /// <summary>
        /// Triggers FadeOut animation in ToastControl
        /// </summary>
        public bool IsRemoving { get; set; }

        public ICommand CloseCommand { get; private set; }

        public TimeSpan SlideAnimationDuration = (TimeSpan)App.Current.FindResource("ToastMoveUpBeginTime");
        public Duration FadeAnimationDuration = (Duration)App.Current.FindResource("ToastFadeOutDuration");

        public ToastControlViewModel(string title, string message, Icons icon) {
            Title = title;
            Message = message;
            Icon = IconHelper.GetIconPath(icon);
            IconTintColor = GetTintColor(icon);

            CloseCommand = new RelayCommand( act => { 
                App.ToastManager.CloseToastNotification(this); 
                App.SoundPlayer.Stop();
            });
        }

        private SolidColorBrush GetTintColor(Icons icon) {
            switch (icon) {
                case Icons.timer:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#6bb347");  // Green
                case Icons.stopwatch:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#4784b3");  // Light Blue
                case Icons.alarm:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#b37f47");  // Orange
                case Icons.error:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#d23429");  // Red
                case Icons.clock:                
                case Icons.info:
                default:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#000000");  // Black
            }
        }
    }
}
