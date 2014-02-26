using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFTrackServiceWebRole
{
    [DataContract]
    public class TrackLocation
    {
        [DataMember]
        public double Latitude { set; get; }
        [DataMember]
        public double Longitude { set; get; }

        [DataMember]
        public int TrackNo { set; get; }
    }
}