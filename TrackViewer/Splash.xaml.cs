using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TrackViewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Splash : Page
    {
        public Splash()
        {
            this.InitializeComponent();
            CheckUserAccountStatus();
        }

        private async void CheckUserAccountStatus()
        {
            try
            {
                var result = await ProxyTracker.GetInstance().Client.IsUserRegisteredAsync(ProxyTracker.GetInstance().GetDeviceId().ToString());
                if (result == true)
                {
                    var activated = await ProxyTracker.GetInstance().Client.IsUserActivatedAsync(ProxyTracker.GetInstance().GetDeviceId().ToString());
                    if (activated == true)
                    {
                        this.Frame.Navigate(typeof(TrackMap));
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(Registration));
                    }
                }
                else
                    this.Frame.Navigate(typeof(Registration));
            }
            catch (Exception ex) { SetMessage(MessageType.Error,"Some error occured, please check your internet connection or try again later");}
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
                    txtMessage.Foreground = new SolidColorBrush(Colors.DarkGreen);
                    break;
                default:
                    break;

            }
            HideMessage();

        }

        private void HideMessage()
        {

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_HideMessage;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        async void timer_HideMessage(object sender, object e)
        {
            txtMessage.Text = "";
            var timer = (DispatcherTimer)sender;
            timer.Stop();

        }
    }
}
