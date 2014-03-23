using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TrackViewerWP.Resources;
using Windows.Devices.Geolocation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Phone.Controls.Maps;
using TrackViewerWP.Services.TrackService;

namespace TrackViewerWP
{
    public partial class MainPage : PhoneApplicationPage
    {



        private Geolocator _geolocator = null;
        private CancellationTokenSource _cts = null;
        CancellationToken token;
        LocationIcon10m _locationIcon10m;
        LocationIcon100m _locationIcon100m;
        DispatcherTimer myTimer;
        DispatcherTimer trackerTimer;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _geolocator = new Geolocator();
            MapCurrentLocation();
            _locationIcon10m = new LocationIcon10m();
            _locationIcon100m = new LocationIcon100m();
            if (myTimer != null) myTimer.Stop();
            if (trackerTimer != null) trackerTimer.Stop();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }
        void MapCurrentLocation()
        {
            // Change the state of our buttons.

            
            try
            {
                SetCurrentLocation();

             
                // Display the location information in the textboxes.
                //   LatitudeTextbox.Text = pos.Coordinate.Latitude.ToString();
                //  LongitudeTextbox.Text = pos.Coordinate.Longitude.ToString();
                //  AccuracyTextbox.Text = pos.Coordinate.Accuracy.ToString();
            }
            catch (System.UnauthorizedAccessException)
            {
                //MessageTextbox.Text = "Location disabled.";

                //LatitudeTextbox.Text = "No data";
                //LongitudeTextbox.Text = "No data";
                //AccuracyTextbox.Text = "No data";
            }
            catch (TaskCanceledException)
            {
                //     MessageTextbox.Text = "Operation canceled.";
            }
            finally
            {
                _cts = null;
            }

            // Reset the buttons.
            //MapLocationButton.IsEnabled = true;
            //CancelGetLocationButton.IsEnabled = false;
        }

        void Client_StartTrackingCompleted(object sender, Services.TrackService.StartTrackingCompletedEventArgs e)
        {

            long trackNo = (long)e.Result;
            if (trackNo > 0)
            {
                ProxyTracker.GetInstance().Client.GetUserInfoCompleted += Client_GetUserInfoCompleted;
                ProxyTracker.GetInstance().Client.GetUserInfoAsync(ProxyTracker.GetInstance().GetDeviceId());
                txtMyTrackId.Text = "Your TrackViewer ID is " + trackNo.ToString();

                txtTrackId.Visibility = Visibility.Visible;
                btnTrack.Visibility = Visibility.Visible;
                txtEnterUserTrackID.Visibility = Visibility.Visible;
                chkTracking.Visibility = Visibility.Visible;
                txtMyTrackId.Visibility = Visibility.Visible;
                imgLoading.Visibility = Visibility.Collapsed;
                ProxyTracker.GetInstance().MyTrackId = Convert.ToInt64(trackNo);

                PostTrackingInfo();
                FetchTrackingInfo();
            }
        }

        void Client_GetUserInfoCompleted(object sender, GetUserInfoCompletedEventArgs e)
        {
            try { 
            ProxyTracker.GetInstance().Name = e.Result.Name;

            txtWelcome.Text = "Welcome "+e.Result.Name+", you're connected!";
            }
            catch { }
        
        }


        async void SetCurrentLocation()
        {

            try
            {
                if (!btnTrack.Content.Equals("➤")) return;

                foreach (var children in trvMap.Children)
                {

                    if (children.GetType().Name == "LocationIcon100m")
                    {
                        trvMap.Children.Remove(children);
                        break;
                    }

                }

                // Get the cancellation token.
                _cts = new CancellationTokenSource();
                token = _cts.Token;
                // Get the location.

                var asyncResult = _geolocator.GetGeopositionAsync();
                var task = asyncResult.AsTask();

                var readyTask = await Task.WhenAny(task, Task.Delay(10000));
                if (readyTask != task)
                {
                    SetMessage(MessageType.Error, "❎ Unable to find your location, trying again...");
                    SetCurrentLocation();
                }

                Geoposition pos = await task;
                //Geoposition pos = await _geolocator.GetGeopositionAsync().AsTask(token);
                //  MessageTextbox.Text = "";

                System.Device.Location.GeoCoordinate location = new System.Device.Location.GeoCoordinate(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                ProxyTracker.GetInstance().MyTrackLocation = new Services.TrackService.TrackLocation { Latitude = pos.Coordinate.Latitude, Longitude = pos.Coordinate.Longitude };

                // Now set the zoom level of the map based on the accuracy of our location data.
                // Default to IP level accuracy. We only show the region at this level - No icon is displayed.
                double zoomLevel = 13.0f;

                //// if we have GPS level accuracy
                Callout callout = new Callout();

                callout.Text = "My Location";
                callout.Lon = "Lon (λ): " + location.Longitude.ToString().Substring(0, 7);
                callout.Lat = "Lat (φ): " + location.Latitude.ToString().Substring(0, 7);
                _locationIcon100m.DataContext = callout;
                // Add the 100m icon and zoom a little closer.
                trvMap.Children.Add(_locationIcon100m);

                MapLayer.SetPosition(_locationIcon100m, location);
                zoomLevel = 17.0f;

                // Set the map to the given location and zoom level.
                trvMap.SetView(location, zoomLevel);

                try
                {
                    ProxyTracker.GetInstance().Client.StartTrackingCompleted += Client_StartTrackingCompleted;
                    ProxyTracker.GetInstance().Client.StartTrackingAsync(ProxyTracker.GetInstance().GetDeviceId(), ProxyTracker.GetInstance().MyTrackLocation);

                }
                catch (Exception ex) { MapCurrentLocation(); }

            }
            catch { }
        }

        private void PostTrackingInfo()
        {
            timer_Tick(null, null);
            myTimer = new DispatcherTimer();
            myTimer.Tick += timer_Tick;
            myTimer.Interval = TimeSpan.FromSeconds(60);
            myTimer.Start();

        }

        async void timer_Tick(object sender, object e)
        {
            try
            {
                if (chkTracking.IsChecked == true)
                {
                    Geoposition pos = await _geolocator.GetGeopositionAsync().AsTask(token);
                    System.Device.Location.GeoCoordinate location = new System.Device.Location.GeoCoordinate(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                    ProxyTracker.GetInstance().MyTrackLocation = new Services.TrackService.TrackLocation { Latitude = location.Latitude, Longitude = location.Longitude };
                    ProxyTracker.GetInstance().Client.PublishTrackingInfoAsync(ProxyTracker.GetInstance().MyTrackId, ProxyTracker.GetInstance().MyTrackLocation);
                }

                if (!btnTrack.Content.ToString().Equals("❌"))
                {
                    SetCurrentLocation();
                }
            }
            catch { }
        }



        private void FetchTrackingInfo()
        {
            timer_TickFetch(null, null);
            trackerTimer = new DispatcherTimer();
            trackerTimer.Tick += timer_TickFetch;
            trackerTimer.Interval = TimeSpan.FromSeconds(60);
            trackerTimer.Start();
        }

        void timer_TickFetch(object sender, object e)
        {
            try
            {
                long trackId = 0;

                if (txtTrackId.Text != "" && btnTrack.Content.ToString().Equals("❌"))
                {
                    ProxyTracker.GetInstance().Client.GetTrackingInfoCompleted += Client_GetTrackingInfoCompleted;
                    if (Int64.TryParse(txtTrackId.Text, out trackId))
                    {
                        ProxyTracker.GetInstance().Client.GetTrackingInfoAsync(Convert.ToInt64(txtTrackId.Text));
                    }
                    else
                    {
                        SetMessage(MessageType.Error, "❎ TrackViewer ID entered is invalid");
                        btnTrack_Click(sender, null);
                    }


                }
            }
            catch { }
        }

        void Client_GetTrackingInfoCompleted(object sender, Services.TrackService.GetTrackingInfoCompletedEventArgs e)
        {
            try
            {
                var res = e.Result;
                if (res != null)
                    SetUserTrackCurrentLocation(res.Latitude, res.Longitude);
                else
                {
                    SetMessage(MessageType.Error, "❎ Sorry, there is no location associated with the user's TrackViewer ID");
                    btnTrack_Click(sender, null);
                }
            }
            catch
            {
                SetMessage(MessageType.Error, "❎ Sorry, there is no location associated with the user's TrackViewer ID");
                btnTrack_Click(sender, null);
            }
        }
        void SetUserTrackCurrentLocation(double latitude, double longitude)
        {
            try
            {
                if (btnTrack.Content.Equals("➤")) return;

                foreach (var children in trvMap.Children)
                {
                    if (children.GetType().Name == "LocationIcon10m")
                    {
                        trvMap.Children.Remove(children);
                        break;
                    }

                }
                // Get the cancellation token.
                _cts = new CancellationTokenSource();
                token = _cts.Token;
                // Get the location.

                CloseMessage();
                System.Device.Location.GeoCoordinate location = new System.Device.Location.GeoCoordinate(latitude, longitude);

                // Now set the zoom level of the map based on the accuracy of our location data.
                // Default to IP level accuracy. We only show the region at this level - No icon is displayed.
                double zoomLevel = 17.0f;
                LocationIcon10m _locationUserIcon10m = new LocationIcon10m();
                Callout callout = new Callout();

                callout.Text = "Tracker's Location";
                callout.Lon = "Lon (λ): " + location.Longitude.ToString().Substring(0, 7); ;
                callout.Lat = "Lat (φ): " + location.Latitude.ToString().Substring(0, 7); ;
                _locationUserIcon10m.DataContext = callout;
                // Add the 10m icon and zoom closer.
                trvMap.Children.Add(_locationUserIcon10m);
                MapLayer.SetPosition(_locationUserIcon10m, location);
                // Set the map to the given location and zoom level.
                trvMap.SetView(location, zoomLevel);
            }
            catch { }
        }

        private void btnTrack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnTrack.Content.Equals("➤"))
                {
                    if (txtTrackId.Text.Trim() == "") { SetMessage(MessageType.Warning, "❎ Please enter valid TrackViewer ID"); return; }
                    btnTrack.Content = "❌";
                    txtTrackId.IsEnabled = false;
                    ShowMessage("Searching Tracker's location...");
                    timer_TickFetch(null, null);
                }
                else
                {
                    btnTrack.Content = "➤";
                    txtTrackId.IsEnabled = true;
                    SetCurrentLocation();
                    foreach (var children in trvMap.Children)
                    {
                        if (children.GetType().Name == "LocationIcon10m")
                        {
                            trvMap.Children.Remove(children);
                        }
                    }
                }
            }
            catch { }
        }
        private void SetMessage(MessageType messageType, String message)
        {
            txtMessage.Text = message;
            //switch (messageType)
            //{
            //    case MessageType.Warning:
            //        txtMessage.Foreground = new SolidColorBrush(Colors.Yellow);
            //        break;
            //    case MessageType.Error:
            //        txtMessage.Foreground = new SolidColorBrush(Colors.Red);
            //        break;
            //    case MessageType.Information:
            //        txtMessage.Foreground = new SolidColorBrush(Colors.Green);
            //        break;
            //    default:
            //        break;

            //}
            HideMessage();

        }

        private void HideMessage()
        {

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_HideMessage;
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Start();
        }

        void timer_HideMessage(object sender, object e)
        {
            txtMessage.Text = "";
            var timer = (DispatcherTimer)sender;
            timer.Stop();
        }

        private void ShowMessage(String message)
        {
            txtMessage.Text = message;
        //    txtMessage.Foreground = new SolidColorBrush(Colors.Green);

        }

        private void CloseMessage() { txtMessage.Text = ""; }

        private void ApplicationBarMenuItemPrivacyPolicy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We can store any information you enter or provide to us in any other way. The types of information collected may include your name, email address, device Id and the track information. As the application is network-capable and a real-time in nature, we uses to push and pull data to/from our service.", "Privacy Policy", MessageBoxButton.OK);
        }

        private void ApplicationBarMenuItemHelp_Click(object sender, EventArgs e)
        {
            
            this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItemDeactivateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Are you sure to deactivate your account. Next time when you reopen the application, it will ask you to register again?", "Confirmation", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    ProxyTracker.GetInstance().Client.DeactivateUserAccountCompleted += Client_DeactivateUserAccountCompleted;
                    ProxyTracker.GetInstance().Client.DeactivateUserAccountAsync(ProxyTracker.GetInstance().GetDeviceId());
                }
            }
            catch { MessageBox.Show("❎ Sorry, we couldnt process your request at this time. Please check your internet connection or try again later", "Error", MessageBoxButton.OK); }
         }

        void Client_DeactivateUserAccountCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("✔ Account has been deactivated successfully", "Information", MessageBoxButton.OK);
            this.NavigationService.Navigate(new Uri("/Splash.xaml", UriKind.Relative));
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            timer_TickFetch(null, null);
            timer_Tick(null, null);
            SetMessage(MessageType.Information, "✔ Location syncing has been done successfully");
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }


    public enum MessageType
    {

        Warning, Information, Error
    }
}