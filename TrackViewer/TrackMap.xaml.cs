using Bing.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using TrackViewer.Services.TrackService;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
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
        DispatcherTimer myTimer;
        DispatcherTimer trackerTimer;
        public TrackMap()
        {
            this.InitializeComponent();
            _geolocator = new Geolocator();
            _locationIcon10m = new LocationIcon10m();
            _locationIcon100m = new LocationIcon100m();
            if (myTimer != null) myTimer.Stop();
            if (trackerTimer != null) trackerTimer.Stop();
            MapCurrentLocation();
            
            AddDeactivateAccountMenu();
        }

        void AddDeactivateAccountMenu()
        {
            SettingsPane.GetForCurrentView().CommandsRequested += (s, e) =>
            {
                SettingsCommand deactivateCommand = new SettingsCommand("deactivate", "Deactivate Account",
                    (handler) =>
                    {
                        DeactivateAccount da = new DeactivateAccount();
                        da.Show();
                    });
                e.Request.ApplicationCommands.Add(deactivateCommand);
            };
        }

        async void MapCurrentLocation()
        {
            try
            {


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
                        Task<TrackViewerUser> trackUser = ProxyTracker.GetInstance().Client.GetUserInfoAsync(ProxyTracker.GetInstance().GetDeviceId());
                        ProxyTracker.GetInstance().Name = trackUser.Result.Name;
                        txtWelcome.Text = "Welcome " + ProxyTracker.GetInstance().Name + ", you're connected!";
                        txtMyTrackId.Text = "Your TrackViewer ID is " + trackNo.ToString();
                        EnableControls();



                        ProxyTracker.GetInstance().MyTrackId = Convert.ToInt64(trackNo);

                        PostTrackingInfo();
                        FetchTrackingInfo();

                        //var trackLocation = ProxyTracker.GetInstance().MyTrackLocation;
                        //SetPushPin("↓", new Location { Latitude = trackLocation.Latitude, Longitude = trackLocation.Longitude });
                    }
                    catch (Exception ex)  {  MapCurrentLocation(); }

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
            catch { }
        }


        private void SetPushPin(String text, Location location)
        {
            Pushpin pushPin = new Pushpin();
            pushPin.Text = text;
            pushPin.Width = 100;
            pushPin.Height = 100;
            pushPin.Tapped += pushPin_Tapped;
            MapLayer.SetPosition(pushPin, location);
            trvMap.Children.Add(pushPin);
        }

        async void pushPin_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }


    
        void EnableControls()
        {

            toggleSwitch.Visibility = Visibility.Visible;
            txtTrackId.Visibility = Visibility.Visible;
            txtEnterUserTrackID.Visibility = Visibility.Visible;
            btnTrack.Visibility = Visibility.Visible;
            imgLoading.Visibility = Visibility.Collapsed;
            txtSyncNow.Visibility = Visibility.Visible;
            btnSync.Visibility = Visibility.Visible;
            txtEnableTracking.Visibility = Visibility.Visible;
            txtLoading.Visibility = Visibility.Collapsed;
        }

        async Task SetCurrentLocation()
        {
            try
            {


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
                //  MessageTextbox.Text = "";

                Location location = new Location(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                ProxyTracker.GetInstance().MyTrackLocation = new Services.TrackService.TrackLocation { Latitude = pos.Coordinate.Latitude, Longitude = pos.Coordinate.Longitude };
                // Now set the zoom level of the map based on the accuracy of our location data.
                // Default to IP level accuracy. We only show the region at this level - No icon is displayed.
                double zoomLevel = 13.0f;


                Callout callout = new Callout();

                callout.Text = "My Location";
                callout.Lon = "Lon (λ): " + location.Longitude.ToString().Substring(0, 7);
                callout.Lat = "Lat (φ): " + location.Latitude.ToString().Substring(0, 7);
                _locationIcon100m.DataContext = callout;

                foreach (var children in trvMap.Children)
                {
                    if (children.GetType().Name == "LocationIcon100m")
                    {
                        trvMap.Children.Remove(children);
                        break;
                    }

                }

                // Add the 100m icon and zoom a little closer.
                trvMap.Children.Add(_locationIcon100m);

                MapLayer.SetPosition(_locationIcon100m, location);
                zoomLevel = 17.0f;

                // Set the map to the given location and zoom level.
                trvMap.SetView(location, zoomLevel);
              //  txtMessage.Text = "";
            }
            catch (Exception ex)
            {
                SetMessage(MessageType.Error, "❎ Location services is disabled on this computer");
            }

        }


        async void SetUserTrackCurrentLocation(double latitude, double longitude)
        {
            try
            {
               

                CloseMessage();
                //  trvMap.Children.Clear();
                // Get the cancellation token.
                _cts = new CancellationTokenSource();
                token = _cts.Token;
                // Get the location.


                Location location = new Location(latitude, longitude);

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
                double zoomLevel = 17.0f;
                LocationIcon10m _locationUserIcon10m = new LocationIcon10m();
                Callout callout = new Callout();

                callout.Text = "Tracker's Location";
                callout.Lon = "Lon (λ): " + location.Longitude.ToString().Substring(0, 7);
                callout.Lat = "Lat (φ): " + location.Latitude.ToString().Substring(0, 7);
                _locationUserIcon10m.DataContext = callout;

                foreach (var children in trvMap.Children)
                {
                    if (children.GetType().Name == "LocationIcon10m")
                    {
                        trvMap.Children.Remove(children);
                        break;
                    }

                }
                // Add the 10m icon and zoom closer.
                trvMap.Children.Add(_locationUserIcon10m);
                MapLayer.SetPosition(_locationUserIcon10m, location);
                // Set the map to the given location and zoom level.
                trvMap.SetView(location, zoomLevel);
            }
            catch { }
        }
        private void PostTrackingInfo()
        {
            timer_Tick(null, null);
            myTimer = new DispatcherTimer();
            myTimer.Tick += timer_Tick;
            myTimer.Interval = TimeSpan.FromSeconds(2);
            myTimer.Start();
            
        }

        async void timer_Tick(object sender, object e)
        {
            try
            {
                if (toggleSwitch.IsOn)
                {
                    Geoposition pos = await _geolocator.GetGeopositionAsync().AsTask(token);
                    Location location = new Location(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                    ProxyTracker.GetInstance().MyTrackLocation = new Services.TrackService.TrackLocation { Latitude = location.Latitude, Longitude = location.Longitude };
                    await ProxyTracker.GetInstance().Client.PublishTrackingInfoAsync(ProxyTracker.GetInstance().MyTrackId, ProxyTracker.GetInstance().MyTrackLocation);
                }

                if (!btnTrack.Content.ToString().Equals("Cancel"))
                {
                    await SetCurrentLocation();
            //        txtMessage.Text = "";
                }
            }
            catch { }
        }



        private void FetchTrackingInfo() {
            timer_TickFetch(null, null);
            trackerTimer = new DispatcherTimer();
            trackerTimer.Tick += timer_TickFetch;
            trackerTimer.Interval = TimeSpan.FromSeconds(60);
            trackerTimer.Start();
          
        }

        async void timer_TickFetch(object sender, object e)
        {
            try
            {
                long trackId = 0;

                if (txtTrackId.Text != "" && btnTrack.Content.ToString().Equals("Cancel"))
                {

                    if (Int64.TryParse(txtTrackId.Text, out trackId))
                    {
                        var res = await ProxyTracker.GetInstance().Client.GetTrackingInfoAsync(Convert.ToInt64(txtTrackId.Text));
                        if (res != null)
                            SetUserTrackCurrentLocation(res.Latitude, res.Longitude);
                        else
                        {
                            SetMessage(MessageType.Error, "❎ Sorry, there is no location associated with the user's TrackViewer ID");
                            btnTrack_Click(sender, null);
                        }
                    }
                    else
                    {
                        SetMessage(MessageType.Error, "❎ TrackViewer ID entered is invalid");
                        btnTrack_Click(sender, null);
                    }

                }
            }
            catch
            {
                SetMessage(MessageType.Error, "❎ Sorry, there is no location associated with the user's TrackViewer ID");
                btnTrack_Click(sender, null);
            }

        }

        private async void btnTrack_Click(object sender, RoutedEventArgs e)
        {
            
            if (btnTrack.Content.Equals("Track now"))
            {
                if (txtTrackId.Text.Trim() == "") {
                    SetMessage(MessageType.Warning, "❎ Please enter valid TrackViewer ID"); 
                    txtTrackId.Focus(Windows.UI.Xaml.FocusState.Programmatic); 
                    return; 
                }
                btnTrack.Content = "Cancel";
                txtTrackId.IsEnabled = false;
                ShowMessage("Searching Tracker's location...");
                timer_TickFetch(null, null);
                
            }
            else { 
                btnTrack.Content = "Track now";
                txtTrackId.IsEnabled = true;
                try { 
                    await SetCurrentLocation();
                    foreach (var children in trvMap.Children)
                    {
                        if (children.GetType().Name == "LocationIcon10m")
                        {
                            trvMap.Children.Remove(children);
                        }
                    }
                }
                catch {  }
            }
        }

        private void ShowMessage(String message)
        {
            txtMessage.Text = message;
            txtMessage.Foreground = new SolidColorBrush(Colors.DarkGreen);

        }

        private void CloseMessage() { txtMessage.Text = ""; }

        private void SetMessage(MessageType messageType, String message)
        {
            txtMessage.Text = message;
            switch(messageType)
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
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Start();
        }

        async void timer_HideMessage(object sender, object e)
        {
            txtMessage.Text = "";
            
            var timer=(DispatcherTimer) sender;
            timer.Stop();
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            timer_TickFetch(null, null);
            timer_Tick(null, null);
            SetMessage(MessageType.Information, "✔ Location syncing has been done successfully");
        }
    }



    public enum MessageType{

        Warning, Information, Error
    }
}
