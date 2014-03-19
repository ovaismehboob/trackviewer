using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace TrackViewer
{
    public sealed partial class DeactivateAccount : SettingsFlyout
    {
        public DeactivateAccount()
        {
            this.InitializeComponent();
        }

        private async void btnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            if (!ProxyTracker.GetInstance().IsTestAccount)
            {
                btnDeactivate.IsEnabled = false;
                await ProxyTracker.GetInstance().Client.DeactivateUserAccountAsync(ProxyTracker.GetInstance().GetDeviceId());
                btnDeactivate.IsEnabled = true;
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Splash));
                RemoveDeactivateAccountMenu();
            }
            else
            {
                txtMessage.Text = "Its a test account, you cannot perform this operation";
            }
        }

        void RemoveDeactivateAccountMenu()
        {
            SettingsPane.GetForCurrentView().CommandsRequested += (s, e) =>
            {
                e.Request.ApplicationCommands.RemoveAt(e.Request.ApplicationCommands.Count()-1);
               
            };

            SettingsPane.Show();
        }



    }
}
