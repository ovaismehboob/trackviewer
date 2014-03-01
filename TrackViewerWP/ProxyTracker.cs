using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace TrackViewerWP
{
    public class ProxyTracker
    {
        private static ProxyTracker _instance = null;
        private static Object _lock = new object();
        Services.TrackService.TrackServiceClient client = null;
        
        private ProxyTracker()
        {

            System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress("http://trackviewerservice.cloudapp.net/TrackService.svc");// new System.ServiceModel.EndpointAddress("http://127.0.0.1:81/TrackService.svc");
            System.ServiceModel.BasicHttpBinding binding =new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);

            client = new Services.TrackService.TrackServiceClient(binding, address);

        }

        public static ProxyTracker GetInstance()
        {
            lock (_lock) { 
             return _instance = (_instance == null) ? new ProxyTracker() : _instance;
            }
        }


        public long MyTrackId
        {

            set;
            get;
        }

        public Services.TrackService.TrackLocation MyTrackLocation
        {

            set;
            get;
        }

        public Services.TrackService.TrackServiceClient Client
        {
            get{ return client;}
        }

        public String GetDeviceId()
        {
            string deviceSerial = string.Empty;

            return deviceSerial;


        }


    }
}
