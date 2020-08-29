using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TimeLine.Views
{
    /// <summary>
    /// Interaction logic for ToastView.xaml
    /// </summary>
    public partial class ToastView : Window
    {
        private bool IsClosing;

        public ToastView() {
            //TODO: Slide in inside window.
            this.Left = WpfScreenHelper.Screen.PrimaryScreen.Bounds.Right;
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e) {
            if (IsClosing == false) {
                e.Cancel = true;
                var animation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
                animation.Completed += (s, _) =>
                {
                    IsClosing = true;
                    base.OnClosing(e);
                    this.Close();
                };

                this.BeginAnimation(UIElement.OpacityProperty, animation);                
            }
        }
    }
}
