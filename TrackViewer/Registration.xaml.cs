using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class Registration : Page
    {
        public Registration()
        {
            this.InitializeComponent();
        }

        private void btnSendActivation_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.Trim() != "" && txtEmailAddress.Text.Trim() != "")
            {
                Task<long> result=ProxyTracker.GetInstance().Client.RegisterUserAsync(ProxyTracker.GetInstance().GetDeviceId(), "", txtName.Text.Trim(), txtEmailAddress.Text.Trim());
                if(result.Result>0){
                    this.Frame.Navigate(typeof(TrackMap));
                }
                else
                {
                    SetMessage(MessageType.Error, "Some error occured, please try again later");
                }
            }
            else
            {
                SetMessage(MessageType.Error, "Name and Email Address cannot be empty, please enter correct information");
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
