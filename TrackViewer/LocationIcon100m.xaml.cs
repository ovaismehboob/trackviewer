using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TrackViewer
{
    public sealed partial class LocationIcon100m : UserControl
    {
        public LocationIcon100m()
        {
            this.InitializeComponent();
        }

    }

    public class Callout : INotifyPropertyChanged
    {

        String text;

        public String Text
        {
            set { this.text = value; OnPropertyChanged("Text"); }
            get { return text; }
        }

        String lon;
        public String Lon
        {
            set { this.lon = value; OnPropertyChanged("Lon"); }
            get { return lon; }
        }

        String lat;
        public String Lat
        {
            set { this.lat = value; OnPropertyChanged("Lat"); }
            get { return lat; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
}