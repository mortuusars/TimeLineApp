using System.Windows;
using TimeLine.Views;

namespace TimeLine
{
    public class ToastManager
    {
        private Window toastHolderView;
        private ToastHolderViewModel toastHolderViewModel;

        public ToastManager() {

        }

        private void CreateTostHolder() {
            if (toastHolderView == null) {
                toastHolderViewModel = new ToastHolderViewModel(this);
                toastHolderViewModel.LastToastClosed += CloseHolderOnLastToastClosed;
                toastHolderView = new ToastHolderView() { DataContext = toastHolderViewModel, Owner = MainView.Main };

                toastHolderView.Show();
            }
        }

        private void CloseHolderOnLastToastClosed(object sender, System.EventArgs e) {
            if (toastHolderView != null) {
                toastHolderView.Close();
                toastHolderView = null;
            }

            toastHolderViewModel = null;
        }


        /// <summary>
        /// Shows Toast Notification in the corner. It will automatically close after short delay. 
        /// IsAlarm = true - plays a sound and makes toast stay open indefinitely.
        /// </summary>
        /// <param name="IsAlarm">Plays sound and makes toast stay until manually closed.</param>
        public void ShowToastNotification(string title, string message, Icons icon, bool IsAlarm = false) {
            if (toastHolderView == null || toastHolderViewModel == null)
                CreateTostHolder();

            toastHolderViewModel.ShowToast(title, message, icon, IsAlarm);
        }

        /// <summary>
        /// Closes Toast with fading animation.
        /// </summary>
        /// <param name="toast"></param>
        /// <param name="fromCommand"></param>
        public void CloseToastNotification(ToastControlViewModel toast, bool fromCommand = false) {
            toastHolderViewModel.CloseToast(toast);
        }
    }
}
