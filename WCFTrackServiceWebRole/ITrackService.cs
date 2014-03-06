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
        [WebInvoke(ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/starttracking/{deviceId}")]
        Int64 StartTracking(String deviceId, TrackLocation location);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/publishtracking")]
        void PublishTrackingInfo(Int64 trackId, TrackLocation location);

        [OperationContract]
        Int64 StopTracking(Int64 trackId);

        [OperationContract]
        TrackLocation GetTrackingInfo(Int64 trackId);
        // TODO: Add your service operations here

        //Restful Methods
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/starttrackingrestful/{deviceId}/{latitude}/{longitude}/{trackNo}")]
        Int64 StartTrackingRestful(String deviceId, String latitude, String longitude, String trackNo);

        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/publishtrackingrestful/{latitude}/{longitude}/{trackNo} ")]
        void PublishTrackingInfoRestful(String latitude, String longitude, String trackNo);

        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/gettrackinginforestful/{trackId}")]
        TrackLocation GetTrackingInfoRestful(String trackId);
        
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/IsUserRegistered/{deviceId}")]
        bool IsUserRegistered(String deviceId);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/RegisterUser/{deviceId}/{activationCode}/{name}/{emailAddress}")]
        Int64 RegisterUser(String deviceId, String activationCode, String name, String emailAddress);


        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "/UpdateIsActivated/{deviceId}")]
        void UpdateIsActivated(String deviceId);
        
    }


}
