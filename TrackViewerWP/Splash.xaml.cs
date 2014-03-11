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
                SetMessage(MessageType.Error, "Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
                btnRetry.Visibility = Visibility.Visible;
            }
        }

        void Client_IsUserRegisteredCompleted(object sender, Services.TrackService.IsUserRegisteredCompletedEventArgs e)
        {
            try
            {
                if ((bool)e.Result == true)
                {

                    ProxyTracker.GetInstance().Client.IsUserActivatedCompleted += Client_IsUserActivatedCompleted;
                    ProxyTracker.GetInstance().Client.IsUserActivatedAsync(ProxyTracker.GetInstance().GetDeviceId().ToString());

                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/Registration.xaml", UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                SetMessage(MessageType.Error, "Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
                btnRetry.Visibility = Visibility.Visible;
                
            }
        }

        void Client_IsUserActivatedCompleted(object sender, Services.TrackService.IsUserActivatedCompletedEventArgs e)
        {
            try
            {
                if (e.Result == true)
                {
                    this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/Registration.xaml", UriKind.Relative));
                }
            }
            catch (Exception ex) {
                SetMessage(MessageType.Error, "Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
                btnRetry.Visibility = Visibility.Visible;
            }
        }

        private void SetMessage(MessageType messageType, String message)
        {
            txtMessage.Text = message;
            switch (messageType)
            {
                case MessageType.Warning:
                    txtMessage.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case MessageType.Error:
                    txtMessage.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case MessageType.Information:
                    txtMessage.Foreground = new SolidColorBrush(Colors.White);
                    break;
                default:
                    break;

            }
        }

        private void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            CheckAccountStatus();
            txtMessage.Text = "Authenticating device and loading user information, please wait...";
            txtMessage.Foreground = new SolidColorBrush(Colors.White);
            btnRetry.Visibility = Visibility.Collapsed;
        }
    }
}