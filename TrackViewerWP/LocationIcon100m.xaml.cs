using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;

namespace TrackViewerWP
{
    public partial class LocationIcon100m : UserControl
    {
        public LocationIcon100m()
        {
            InitializeComponent();
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
