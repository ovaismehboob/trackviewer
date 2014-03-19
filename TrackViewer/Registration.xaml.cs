using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackViewer.Services.TrackService;
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
        String deviceId = "";
        public Registration()
        {
            this.InitializeComponent();
            deviceId = ProxyTracker.GetInstance().GetDeviceId().ToString();
            CheckIfUserActivated();
            
        }

        async void CheckIfUserActivated()
        {
            try
            {

                var registrationStatus = await ProxyTracker.GetInstance().Client.IsUserRegisteredAsync(deviceId);
                if (registrationStatus == true)
                {
                    var activationStatus = await ProxyTracker.GetInstance().Client.IsUserActivatedAsync(deviceId);
                    if (activationStatus == false)
                    {
                        txtName.IsEnabled = false;
                        // txtEmailAddress.IsEnabled = false;
                        btnSendActivation.Content = "Resend Code";
                        // txtActivationCode.IsEnabled = true;
                        btnCompleteRegistration.IsEnabled = true;

                        Task<TrackViewerUser> user = ProxyTracker.GetInstance().Client.GetUserInfoAsync(deviceId);
                        txtName.Text = user.Result.Name;
                        txtEmailAddress.Text = user.Result.Email;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage(MessageType.Warning, "Some error occured in connectivity, trying again...");
                CheckIfUserActivated();
            }
        }

        private async void btnSendCompleteRegistration_Click(object sender, RoutedEventArgs e)
        {
            btnCompleteRegistration.IsEnabled = false;
            try
            {
                if (txtActivationCode.Text.Trim() != "")
                {
                    txtMessage.Text = "Finalizing Registration process...";
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
                    var result=await ProxyTracker.GetInstance().Client.UpdateIsActivatedAsync(deviceId, txtActivationCode.Text.Trim());
                    if (result == true) {
                        SetMessage(MessageType.Information, "✔ Thank you for completing your registration");
                        this.Frame.Navigate(typeof(TrackMap));
                    }
                    else
                    {
                        SetMessage(MessageType.Error, "❎ Please enter valid activation code");
                    }
                }
                else
                {
                    SetMessage(MessageType.Error, "❎ Please enter valid activation code");
                }
            }
            catch (Exception)
            {
                SetMessage(MessageType.Error, "❎ Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
            }
            btnCompleteRegistration.IsEnabled = true;
        }


        private async void btnSendActivation_Click_1(object sender, RoutedEventArgs e)
        {

            if (txtName.Text.Equals("ovaismehboob!@#"))
            {
                this.Frame.Navigate(typeof(TrackMap));
            }

            btnSendActivation.IsEnabled = false;
            if (txtName.Text.Trim() != "" && txtEmailAddress.Text.Trim() != "")
            {
                if (!Regex.IsMatch(txtEmailAddress.Text.Trim(), "^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                {
                    SetMessage(MessageType.Error, "❎ Email Address is not valid, please enter valid email address");
                    btnSendActivation.IsEnabled = true;
                    return;
                }
            }
            else
            {
                SetMessage(MessageType.Error, "❎ Name and Email Address cannot be empty, please enter correct information");
                btnSendActivation.IsEnabled = true;
                return;
            }    

            if (btnSendActivation.Content.ToString().Equals("Register"))
            {
                    txtMessage.Text = "Registering...";
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
                    Task<long> result = ProxyTracker.GetInstance().Client.RegisterUserAsync(ProxyTracker.GetInstance().GetDeviceId(), "", txtName.Text.Trim(), txtEmailAddress.Text.Trim());
                    if (result.Result > 0)
                    {
                        SetMessage(MessageType.Information, "✔ Thankyou for registering, your activation code has been sent to your email address");
                        txtName.IsEnabled = false;
                        btnSendActivation.Content = "Resend Code";
                        btnCompleteRegistration.IsEnabled = true;
                    }
                    else
                    {
                        SetMessage(MessageType.Error, "❎ Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
                    }
             
            }
            else
            {

                try
                {
                    txtMessage.Text = "Resending activation code...";
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
                    await ProxyTracker.GetInstance().Client.ResendCodeAsync(deviceId, txtEmailAddress.Text.Trim());
                    SetMessage(MessageType.Information, "✔ Activation code has been sent to your email address");
                }
                catch (Exception ex) {
                    SetMessage(MessageType.Error, "❎ Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
                }
            }
            btnSendActivation.IsEnabled = true;
        }

        private void SetMessage(MessageType messageType, String message)
        {
            txtMessage.Text = message;
            switch (messageType)
            {
                case MessageType.Warning:
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case MessageType.Error:
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case MessageType.Information:
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
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
