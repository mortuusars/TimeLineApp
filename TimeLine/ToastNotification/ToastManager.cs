using System.Windows;
using TimeLine.Views;

namespace TimeLine
{
    public class ToastManager
    {
        private Window toastHolder;
        private ToastHolderViewModel toastHolderViewModel;

        public ToastManager() {

            toastHolderViewModel = new ToastHolderViewModel();
            toastHolder = new ToastHolderView() { DataContext = toastHolderViewModel };            
            
            toastHolder.Show();
        }


        /// <summary>
        /// Shows Toast Notification in the corner. It will automatically close after short delay. 
        /// IsAlarm = true - plays a sound and makes toast stay open indefinitely.
        /// </summary>
        /// <param name="IsAlarm">Plays sound and makes toast stay until manually closed.</param>
        public void ShowToastNotification(string title, string message, Icons icon, bool IsAlarm = false) {

            toastHolderViewModel.ShowToast(title, message, icon, IsAlarm);
        }

        public void CloseToastNotification(ToastControlViewModel toast, bool fromCommand = false) {
            toastHolderViewModel.CloseToast(toast);
        }
    }
}
