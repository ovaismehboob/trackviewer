using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace TrackViewer
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


        public bool IsTestAccount
        {
            get;
            set;
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

        public String Name { set; get; }

        public String GetDeviceId()
        {

            if (IsTestAccount) return "test";
            else
            {
                string deviceSerial = string.Empty;

                //Windows.System.Profile.HardwareToken hardwareToken = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
                //using (DataReader dataReader = DataReader.FromBuffer(hardwareToken.Id))
                //{
                //    int offset = 0;
                //    while (offset < hardwareToken.Id.Length)
                //    {
                //        byte[] hardwareEntry = new byte[4];
                //        dataReader.ReadBytes(hardwareEntry);

                //        // CPU ID of the processor || Size of the memory || Serial number of the disk device || BIOS
                //        if ((hardwareEntry[0] == 1 || hardwareEntry[0] == 2 || hardwareEntry[0] == 3 || hardwareEntry[0] == 9) && hardwareEntry[1] == 0)
                //        {
                //            if (!string.IsNullOrEmpty(deviceSerial))
                //            {
                //                deviceSerial += "|";
                //            }
                //            deviceSerial += string.Format("{0}.{1}", hardwareEntry[2], hardwareEntry[3]);
                //        }
                //        offset += 4;
                //    }
                //}
                //return deviceSerial;

                var token = HardwareIdentification.GetPackageSpecificToken(null);
                var hardwareId = token.Id;
                var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

                byte[] bytes = new byte[hardwareId.Length];
                dataReader.ReadBytes(bytes);

                deviceSerial = BitConverter.ToString(bytes);
                return deviceSerial;
            }
        }


    }
}
