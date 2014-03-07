using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFTrackServiceWebRole
{
    [DataContract]
    public class TrackViewerUser
    {
        [DataMember]
        public String Name { set; get; }

        [DataMember]
        public String Email { set; get; }

        [DataMember]
        public String ActivationCode { set; get; }

        [DataMember]
        public String DeviceId { set; get; }
    }
}