using Bing.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
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
    public sealed partial class TrackMap : Page
    {


        private Geolocator _geolocator = null;
        private CancellationTokenSource _cts = null;
        LocationIcon10m _locationIcon10m;
        LocationIcon100m _locationIcon100m;
        CancellationToken token;
        public TrackMap()
        {
            this.InitializeComponent();
            _geolocator = new Geolocator();
            _locationIcon10m = new LocationIcon10m();
            _locationIcon100m = new LocationIcon100m();
            MapCurrentLocation();
        }

        async void MapCurrentLocation()
        {
            // Change the state of our buttons.


            // Remove any previous location icon.
            if (trvMap.Children.Count > 0)
            {
                trvMap.Children.RemoveAt(0);
            }

            try
            {
                await SetCurrentLocation();

                try
                {
                    long trackNo = await ProxyTracker.GetInstance().Client.StartTrackingAsync(ProxyTracker.GetInstance().GetDeviceId(), ProxyTracker.GetInstance().MyTrackLocation);
                    txtWelcome.Text = "Welcome, you're connected!";
                    txtMyTrackId.Text = "Your TrackViewer ID is " + trackNo.ToString();
                    EnableControls();
                    ProxyTracker.GetInstance().MyTrackId = Convert.ToInt64(trackNo);

                    PostTrackingInfo();
                    FetchTrackingInfo();
                
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


        void EnableControls()
        {

            toggleSwitch.Visibility = Visibility.Visible;
            txtTrackId.Visibility = Visibility.Visible;
            txtEnterUserTrackID.Visibility = Visibility.Visible;
            btnTrack.Visibility = Visibility.Visible;
            imgLoading.Visibility = Visibility.Collapsed;
        }

        async Task SetCurrentLocation()
        {
            // Get the cancellation token.
            _cts = new CancellationTokenSource();
            token = _cts.Token;
            // Get the location.
            Geoposition pos = await _geolocator.GetGeopositionAsync().AsTask(token);
            //  MessageTextbox.Text = "";

            Location location = new Location(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
            ProxyTracker.GetInstance().MyTrackLocation = new Services.TrackService.TrackLocation { Latitude = pos.Coordinate.Latitude, Longitude = pos.Coordinate.Longitude };

            // Now set the zoom level of the map based on the accuracy of our location data.
            // Default to IP level accuracy. We only show the region at this level - No icon is displayed.
            double zoomLevel = 13.0f;

            // if we have GPS level accuracy
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


        async void SetUserTrackCurrentLocation(double latitude, double longitude)
        {
            // Get the cancellation token.
            _cts = new CancellationTokenSource();
            token = _cts.Token;
            // Get the location.
          

            Location location = new Location(latitude,longitude);

            //// Now set the zoom level of the map based on the accuracy of our location data.
            //// Default to IP level accuracy. We only show the region at this level - No icon is displayed.
            //double zoomLevel = 13.0f;

            //// Add the 10m icon and zoom closer.
            //if (trvMap.Children.Count == 0) { 
            //    trvMap.Children.Add(_locationIcon10m);
            //}

            //MapLayer.SetPosition(_locationIcon10m, location);
            //zoomLevel = 15.0f;
            
            //// Set the map to the given location and zoom level.
            //trvMap.SetView(location, zoomLevel);

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
        private void PostTrackingInfo()
        {

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
            
        }

        async void timer_Tick(object sender, object e)
        {
            if (toggleSwitch.IsOn)
            {
                Geoposition pos = await _geolocator.GetGeopositionAsync().AsTask(token);
                Location location = new Location(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                ProxyTracker.GetInstance().MyTrackLocation = new Services.TrackService.TrackLocation { Latitude = location.Latitude, Longitude = location.Longitude };
                await ProxyTracker.GetInstance().Client.PublishTrackingInfoAsync(ProxyTracker.GetInstance().MyTrackId, ProxyTracker.GetInstance().MyTrackLocation);
            }
        }



        private void FetchTrackingInfo() {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick+=timer_TickFetch;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        async void timer_TickFetch(object sender, object e)
        {

            if (txtTrackId.Text != "" && btnTrack.Content.ToString().Equals("Cancel"))
            {
                var res = await ProxyTracker.GetInstance().Client.GetTrackingInfoAsync(Convert.ToInt64(txtTrackId.Text));
                if(res!=null)
                    SetUserTrackCurrentLocation(res.Latitude, res.Longitude);
            }
        }

        private async void btnTrack_Click(object sender, RoutedEventArgs e)
        {
            if (btnTrack.Content.Equals("Track now"))
            {
                if (txtTrackId.Text.Trim() == "") { SetMessage(MessageType.Warning, "Please enter valid Track Id"); txtTrackId.Focus(Windows.UI.Xaml.FocusState.Programmatic); return; }
                btnTrack.Content = "Cancel";
                txtTrackId.IsEnabled = false;
            }
            else { 
                btnTrack.Content = "Track now";
                txtTrackId.IsEnabled = true;
                try { 
                await SetCurrentLocation();
                }
                catch {  }
            }
        }

        private void SetMessage(MessageType messageType, String message)
        {
            txtMessage.Text = message;
            switch(messageType)
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
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        async void timer_HideMessage(object sender, object e)
        {
            txtMessage.Text = "";
            var timer=(DispatcherTimer) sender;
            timer.Stop();
            
        }
    }



    public enum MessageType{

        Warning, Information, Error
    }
}
