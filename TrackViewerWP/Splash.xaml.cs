using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace TrackViewerWP
{
    public partial class Splash : PhoneApplicationPage
    {
        public Splash()
        {
            InitializeComponent();
            CheckAccountStatus();
        }

        void CheckAccountStatus(){
            try { 
            ProxyTracker.GetInstance().Client.IsUserRegisteredCompleted += Client_IsUserRegisteredCompleted;
            ProxyTracker.GetInstance().Client.IsUserRegisteredAsync(ProxyTracker.GetInstance().GetDeviceId());
            }
            catch (Exception)
            {
                SetMessage(MessageType.Error, "Service is not responding, please check your internet connection or try again later");
            }
        }

        void Client_IsUserRegisteredCompleted(object sender, Services.TrackService.IsUserRegisteredCompletedEventArgs e)
        {
            if ((bool)e.Result == true)
            {
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/Registration.xaml", UriKind.Relative));
            }
        }

        private void SetMessage(MessageType messageType, String message)
        {
            txtMessage.Text = message;
            switch (messageType)
            {
                case MessageType.Warning:
                    txtMessage.Foreground = new SolidColorBrush(Colors.Yellow);
                    break;
                case MessageType.Error:
                    txtMessage.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case MessageType.Information:
                    txtMessage.Foreground = new SolidColorBrush(Colors.Green);
                    break;
                default:
                    break;

            }
        }
    }
}