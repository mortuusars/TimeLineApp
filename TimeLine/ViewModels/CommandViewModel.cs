using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MortuusLogger;
using WpfScreenHelper;

namespace TimeLine.ViewModels
{
    public class CommandViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Left { get; set; }
        public int Top { get; set; }

        public int WindowWidth { get; set; } = 400;
        public int BaseHeight { get; set; } = 70;

        public string Input { get; set; } = string.Empty;


        
        public ICommand ConfirmCommand { get; private set; }
        public ICommand EraseCommand { get; private set; }


        public CommandViewModel() {
            ConfirmCommand = new RelayCommand(param => { ConfirmAndClose(); });
            EraseCommand = new RelayCommand(act => { Input = string.Empty; });


            SetWindowPosition();
            Logger.Log($"CommandWindow Position: x:{Left} : y:{Top}", LogLevel.DEBUG);
        }

        private void ConfirmAndClose() {  
            GetService.Manager.ShowOrCloseCommandView();
            GetService.Manager.ParseInput(Input);
        }



        #region Position

        public void SetWindowPosition() {
            Point position = MouseHelper.MousePosition;
            Screen currentScreen = Screen.FromPoint(position);

            Left = GetPositionX(position, currentScreen);
            Top = GetPositionY(position, currentScreen);
        }

        private int GetPositionX(Point position, Screen currentScreen) {

            // If distance between bounds and mouse position is less than half of the window: position stays around bounds.
            if (Math.Abs(currentScreen.Bounds.Left - position.X) < WindowWidth / 2)
                position.X = currentScreen.Bounds.Left + (WindowWidth / 2) * 1.1;
            else if (Math.Abs(currentScreen.Bounds.Right - position.X) < WindowWidth / 2)
                position.X = currentScreen.Bounds.Right - (WindowWidth / 2) * 1.1;


            return (int)position.X - WindowWidth / 2;
        }

        private int GetPositionY(Point position, Screen currentScreen) {

            if (Math.Abs(currentScreen.Bounds.Top - position.Y) < BaseHeight * 2)
                position.Y = currentScreen.Bounds.Top + BaseHeight * 2;
            else if (Math.Abs(currentScreen.Bounds.Bottom - position.Y) < BaseHeight * 4)
                position.Y = currentScreen.Bounds.Bottom - BaseHeight * 4 * 1.1;

            return (int)position.Y - BaseHeight / 2;
        }

        #endregion
    }
}
