using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFTrackServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ITrackService
    {

        [OperationContract]
        Int64 StartTracking(String deviceId, TrackLocation location);

        [OperationContract]
        void PublishTrackingInfo(Int64 trackId, TrackLocation location);

        [OperationContract]
        Int64 StopTracking(Int64 trackId);

        [OperationContract]
        TrackLocation GetTrackingInfo(Int64 trackId);
        // TODO: Add your service operations here
    }


}
