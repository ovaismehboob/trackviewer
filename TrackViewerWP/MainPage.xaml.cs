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

namespace TrackViewerWP
{
    public partial class MainPage : PhoneApplicationPage
    {



        private Geolocator _geolocator = null;
        private CancellationTokenSource _cts = null;
        CancellationToken token;
        LocationIcon10m _locationIcon10m;
        LocationIcon100m _locationIcon100m;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _geolocator = new Geolocator();
            MapCurrentLocation();
            _locationIcon10m = new LocationIcon10m();
            _locationIcon100m = new LocationIcon100m();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }
        async void MapCurrentLocation()
        {
            // Change the state of our buttons.

            
            try
            {
                await SetCurrentLocation();

                try
                {
                    ProxyTracker.GetInstance().Client.StartTrackingCompleted += Client_StartTrackingCompleted;
                    ProxyTracker.GetInstance().Client.StartTrackingAsync(ProxyTracker.GetInstance().GetDeviceId(), ProxyTracker.GetInstance().MyTrackLocation);
                    
                }
                catch (Exception ex) { MapCurrentLocation(); }

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
            txtWelcome.Text = "Welcome, you're connected!";
            txtMyTrackId.Text = "Your TrackViewer ID is " + trackNo.ToString();

            ProxyTracker.GetInstance().MyTrackId = Convert.ToInt64(trackNo);

            PostTrackingInfo();
            FetchTrackingInfo();

        }


        async Task SetCurrentLocation()
        {
            // Get the cancellation token.
            _cts = new CancellationTokenSource();
            token = _cts.Token;
            // Get the location.
            Geoposition pos = await _geolocator.GetGeopositionAsync().AsTask(token);
            //  MessageTextbox.Text = "";

            System.Device.Location.GeoCoordinate location = new System.Device.Location.GeoCoordinate(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
            ProxyTracker.GetInstance().MyTrackLocation = new Services.TrackService.TrackLocation { Latitude = pos.Coordinate.Latitude, Longitude = pos.Coordinate.Longitude };

            // Now set the zoom level of the map based on the accuracy of our location data.
            // Default to IP level accuracy. We only show the region at this level - No icon is displayed.
            double zoomLevel = 13.0f;

            //// if we have GPS level accuracy
            if (pos.Coordinate.Accuracy <= 10)
            {
                // Add the 10m icon and zoom closer.
                trvMap.Children.Add(_locationIcon10m);
                MapLayer.SetPosition(_locationIcon10m, location);
                zoomLevel = 15.0f;
            }
            // Else if we have Wi-Fi level accuracy.
            else if (pos.Coordinate.Accuracy <= 100)
            {
                // Add the 100m icon and zoom a little closer.
                trvMap.Children.Add(_locationIcon100m);
                MapLayer.SetPosition(_locationIcon100m, location);
                zoomLevel = 14.0f;
            }

            // Set the map to the given location and zoom level.
            trvMap.SetView(location, zoomLevel); 
        }

        private void PostTrackingInfo()
        {

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();

        }

        async void timer_Tick(object sender, object e)
        {
            //if (toggleSwitch.IsOn)
            //{
                Geoposition pos = await _geolocator.GetGeopositionAsync().AsTask(token);
                System.Device.Location.GeoCoordinate location = new System.Device.Location.GeoCoordinate(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                ProxyTracker.GetInstance().MyTrackLocation = new Services.TrackService.TrackLocation { Latitude = location.Latitude, Longitude = location.Longitude };
                ProxyTracker.GetInstance().Client.PublishTrackingInfoAsync(ProxyTracker.GetInstance().MyTrackId, ProxyTracker.GetInstance().MyTrackLocation);
            //}
        }



        private void FetchTrackingInfo()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_TickFetch;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        void timer_TickFetch(object sender, object e)
        {

            if (txtTrackId.Text != "" && btnTrack.Content.ToString().Equals("Cancel"))
            {
                ProxyTracker.GetInstance().Client.GetTrackingInfoCompleted += Client_GetTrackingInfoCompleted;
                ProxyTracker.GetInstance().Client.GetTrackingInfoAsync(Convert.ToInt64(txtTrackId.Text));

            }
        }

        void Client_GetTrackingInfoCompleted(object sender, Services.TrackService.GetTrackingInfoCompletedEventArgs e)
        {
            var res =e.Result;
            if (res != null)
                SetUserTrackCurrentLocation(res.Latitude, res.Longitude);
        }
        void SetUserTrackCurrentLocation(double latitude, double longitude)
        {
            // Get the cancellation token.
            _cts = new CancellationTokenSource();
            token = _cts.Token;
            // Get the location.


            System.Device.Location.GeoCoordinate location = new System.Device.Location.GeoCoordinate(latitude, longitude);

            // Now set the zoom level of the map based on the accuracy of our location data.
            // Default to IP level accuracy. We only show the region at this level - No icon is displayed.
            double zoomLevel = 20.0f;
            LocationIcon10m _locationUserIcon10m = new LocationIcon10m();
            // Add the 10m icon and zoom closer.
            trvMap.Children.Add(_locationUserIcon10m);
            MapLayer.SetPosition(_locationUserIcon10m, location);
            // Set the map to the given location and zoom level.
            trvMap.SetView(location, zoomLevel); 
        }

        private async void btnTrack_Click(object sender, RoutedEventArgs e)
        {
            if (btnTrack.Content.Equals("Track now"))
            {
                if (txtTrackId.Text.Trim() == "") { SetMessage(MessageType.Warning, "Please enter valid Track Id"); txtTrackId.Focus(); return; }
                btnTrack.Content = "Cancel";
                txtTrackId.IsEnabled = false;
            }
            else
            {
                btnTrack.Content = "Track now";
                txtTrackId.IsEnabled = true;
                await SetCurrentLocation();

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

        private void HideMessage()
        {

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_HideMessage;
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();
        }

        async void timer_HideMessage(object sender, object e)
        {
            txtMessage.Text = "";
            var timer = (DispatcherTimer)sender;
            timer.Stop();

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