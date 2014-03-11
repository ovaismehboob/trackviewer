using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using TrackViewerWP.Services.TrackService;

namespace TrackViewerWP
{
    public partial class Registration : PhoneApplicationPage
    {

        String deviceId = "";
        public Registration()
        {
            InitializeComponent();
            deviceId = ProxyTracker.GetInstance().GetDeviceId().ToString();
            CheckIfUserActivated();
        }

        void CheckIfUserActivated()
        {
            try
            {

                ProxyTracker.GetInstance().Client.IsUserRegisteredCompleted += Client_IsUserRegisteredCompleted;
                ProxyTracker.GetInstance().Client.IsUserRegisteredAsync(deviceId);
            }            
            catch (Exception ex)
            {
                ProxyTracker.GetInstance().Client.IsUserRegisteredCompleted -= Client_IsUserRegisteredCompleted;
                SetMessage(MessageType.Warning, "Sorry, we couldn't connect to the service at this time. Trying again...");             
                CheckIfUserActivated();
            }
        }

        void Client_IsUserRegisteredCompleted(object sender, IsUserRegisteredCompletedEventArgs e)
        {
            try
            {
                if (e.Result == true)
                {
                    ProxyTracker.GetInstance().Client.IsUserActivatedCompleted += Client_IsUserActivatedCompleted;
                    ProxyTracker.GetInstance().Client.IsUserActivatedAsync(deviceId);
                }
            }
            catch
            {
                ProxyTracker.GetInstance().Client.IsUserRegisteredCompleted -= Client_IsUserRegisteredCompleted;
                SetMessage(MessageType.Warning, "Sorry, we couldn't connect to the service at this time. Trying again...");
                CheckIfUserActivated();
            }
        }

        void Client_IsUserActivatedCompleted(object sender, IsUserActivatedCompletedEventArgs e)
        {

            if (e.Result == false)
            {
                txtName.IsEnabled = false;
                // txtEmailAddress.IsEnabled = false;
                btnSendActivation.Content = "Resend Code";
                // txtActivationCode.IsEnabled = true;
                btnCompleteRegistration.IsEnabled = true;

                ProxyTracker.GetInstance().Client.GetUserInfoCompleted += Client_GetUserInfoCompleted;
                ProxyTracker.GetInstance().Client.GetUserInfoAsync(deviceId);
            }
        }

        void Client_GetUserInfoCompleted(object sender, GetUserInfoCompletedEventArgs e)
        {
            try
            {
                TrackViewerUser user = (TrackViewerUser)e.Result;
                txtName.Text = user.Name;
                txtEmailAddress.Text = user.Email;
            }
            catch
            {
                
            }
        }


        private void btnSendActivation_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.Trim() != "" && txtEmailAddress.Text.Trim() != "")
            {
                ProxyTracker.GetInstance().Client.RegisterUserCompleted += Client_RegisterUserCompleted;
                ProxyTracker.GetInstance().Client.RegisterUserAsync(ProxyTracker.GetInstance().GetDeviceId(), "", txtName.Text.Trim(), txtEmailAddress.Text.Trim());
                
            }
            else
            {
                SetMessage(MessageType.Error, "Name and Email Address cannot be set empty, please enter complete information");
            }
        }

        void Client_RegisterUserCompleted(object sender, Services.TrackService.RegisterUserCompletedEventArgs e)
        {
            try
            {
                if (e.Result > 0)
                {
                    SetMessage(MessageType.Information, "Thank you for registering, your activation code has been sent to your email address");
                    txtName.IsEnabled = false;
                    btnSendActivation.Content = "Resend Code";
                    btnCompleteRegistration.IsEnabled = true;
                }
                else
                {
                    SetMessage(MessageType.Error, "Sorry, we couldn't process your request at this time. Please check your internet connection or try again later");
                }
            }
            catch
            {
                SetMessage(MessageType.Error, "Sorry, we couldn't process your request at this time. Please check your internet connection or try again later");
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
            HideMessage();

        }

        private void btnSendCompleteRegistration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtActivationCode.Text.Trim() != "")
                {
                    txtMessage.Text = "Finalizing Registration process...";
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
                    ProxyTracker.GetInstance().Client.UpdateIsActivatedCompleted += Client_UpdateIsActivatedCompleted;
                    ProxyTracker.GetInstance().Client.UpdateIsActivatedAsync(deviceId, txtActivationCode.Text.Trim());
                   
                }
                else
                {
                    SetMessage(MessageType.Error, "Please enter valid activation code");
                }
            }
            catch (Exception)
            {
                SetMessage(MessageType.Error, "Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
            }
        }

        void Client_UpdateIsActivatedCompleted(object sender, UpdateIsActivatedCompletedEventArgs e)
        {
            try
            {
                if (e.Result == true)
                {
                    SetMessage(MessageType.Information, "Thank you for completing your registration");
                    this.NavigationService.Navigate(new Uri("/TrackMap", UriKind.Relative));
                }
                else
                {
                    SetMessage(MessageType.Error, "Please enter valid activation code");
                }
            }
            catch
            {
                SetMessage(MessageType.Error, "Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
            }
        }


        private void btnSendActivation_Click_1(object sender, RoutedEventArgs e)
        {
            if (btnSendActivation.Content.ToString().Equals("Register"))
            {
                if (txtName.Text.Trim() != "" && txtEmailAddress.Text.Trim() != "")
                {
                    txtMessage.Text = "Registering...";
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
                    ProxyTracker.GetInstance().Client.RegisterUserCompleted+=Client_RegisterUserCompleted;     
                    ProxyTracker.GetInstance().Client.RegisterUserAsync(ProxyTracker.GetInstance().GetDeviceId(), "", txtName.Text.Trim(), txtEmailAddress.Text.Trim());
                    
                }
                else
                {
                    SetMessage(MessageType.Error, "Name and Email Address cannot be empty, please enter correct information");
                }
            }
            else
            {

                try
                {
                    txtMessage.Text = "Resending activation code...";
                    txtMessage.Foreground = new SolidColorBrush(Colors.Black);
                    ProxyTracker.GetInstance().Client.ResendCodeCompleted += Client_ResendCodeCompleted;
                    ProxyTracker.GetInstance().Client.ResendCodeAsync(deviceId, txtEmailAddress.Text.Trim());
                }
                catch (Exception ex)
                {
                    SetMessage(MessageType.Error, "Sorry, we couldnt process your request at this time. Please check your internet connection or try again later");
                }
            }
        }

        void Client_ResendCodeCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            SetMessage(MessageType.Information, "Activation code has been sent to your email address");
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