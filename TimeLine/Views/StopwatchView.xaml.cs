using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TimeLine.Views
{
    /// <summary>
    /// Interaction logic for StopwatchView.xaml
    /// </summary>
    public partial class StopwatchView : Window
    {
        private bool IsClosing = false;

        public StopwatchView() {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }

        protected override void OnClosing(CancelEventArgs e) {
            
            if (IsClosing)
                e.Cancel = false;
            else
                e.Cancel = true;

            var timer = new DispatcherTimer();
            timer.Interval = ((Duration)App.Current.FindResource("WindowFadeDuration")).TimeSpan;
            timer.Tick += (s, e) => { 
                IsClosing = true; 
                this.Close(); 
                timer.Stop(); 
            };

            timer.Start();
        }
    }
}
