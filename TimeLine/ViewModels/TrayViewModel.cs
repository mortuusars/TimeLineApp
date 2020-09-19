using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace TimeLine.ViewModels
{
    public class TrayViewModel
    {
        public ICommand TrayDoubleClickCommand { get; private set; }

        public ICommand RunCommandWindowCommand { get; private set; }
        public ICommand ConfigCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public TrayViewModel() {
            TrayDoubleClickCommand = new RelayCommand(act => { }); //Empty for now

            RunCommandWindowCommand = new RelayCommand(act => { App.Manager.ToggleRunCommandView(); });
            ConfigCommand = new RelayCommand(act => { MessageBox.Show("Opened config window", "Config", 
                                                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, 
                                                        MessageBoxOptions.ServiceNotification); });
            ExitCommand = new RelayCommand(act => { App.ExitApplication(); });

        }
    }
}
