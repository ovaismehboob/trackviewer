﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.VisualStudio.ServiceReference.Platforms, version 12.0.21005.1
// 
namespace TrackViewer.Services.TrackService {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TrackLocation", Namespace="http://schemas.datacontract.org/2004/07/WCFTrackServiceWebRole")]
    public partial class TrackLocation : object, System.ComponentModel.INotifyPropertyChanged {
        
        private double LatitudeField;
        
        private double LongitudeField;
        
        private int TrackNoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Latitude {
            get {
                return this.LatitudeField;
            }
            set {
                if ((this.LatitudeField.Equals(value) != true)) {
                    this.LatitudeField = value;
                    this.RaisePropertyChanged("Latitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Longitude {
            get {
                return this.LongitudeField;
            }
            set {
                if ((this.LongitudeField.Equals(value) != true)) {
                    this.LongitudeField = value;
                    this.RaisePropertyChanged("Longitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TrackNo {
            get {
                return this.TrackNoField;
            }
            set {
                if ((this.TrackNoField.Equals(value) != true)) {
                    this.TrackNoField = value;
                    this.RaisePropertyChanged("TrackNo");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Services.TrackService.ITrackService")]
    public interface ITrackService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrackService/StartTracking", ReplyAction="http://tempuri.org/ITrackService/StartTrackingResponse")]
        System.Threading.Tasks.Task<long> StartTrackingAsync(string deviceId, TrackViewer.Services.TrackService.TrackLocation location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrackService/PublishTrackingInfo", ReplyAction="http://tempuri.org/ITrackService/PublishTrackingInfoResponse")]
        System.Threading.Tasks.Task PublishTrackingInfoAsync(long trackId, TrackViewer.Services.TrackService.TrackLocation location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrackService/StopTracking", ReplyAction="http://tempuri.org/ITrackService/StopTrackingResponse")]
        System.Threading.Tasks.Task<long> StopTrackingAsync(long trackId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrackService/GetTrackingInfo", ReplyAction="http://tempuri.org/ITrackService/GetTrackingInfoResponse")]
        System.Threading.Tasks.Task<TrackViewer.Services.TrackService.TrackLocation> GetTrackingInfoAsync(long trackId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrackService/StartTrackingRestful", ReplyAction="http://tempuri.org/ITrackService/StartTrackingRestfulResponse")]
        System.Threading.Tasks.Task<long> StartTrackingRestfulAsync(string deviceId, string latitude, string longitude, string trackNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrackService/PublishTrackingInfoRestful", ReplyAction="http://tempuri.org/ITrackService/PublishTrackingInfoRestfulResponse")]
        System.Threading.Tasks.Task PublishTrackingInfoRestfulAsync(string latitude, string longitude, string trackNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrackService/GetTrackingInfoRestful", ReplyAction="http://tempuri.org/ITrackService/GetTrackingInfoRestfulResponse")]
        System.Threading.Tasks.Task<TrackViewer.Services.TrackService.TrackLocation> GetTrackingInfoRestfulAsync(string trackId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITrackServiceChannel : TrackViewer.Services.TrackService.ITrackService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TrackServiceClient : System.ServiceModel.ClientBase<TrackViewer.Services.TrackService.ITrackService>, TrackViewer.Services.TrackService.ITrackService {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public TrackServiceClient() : 
                base(TrackServiceClient.GetDefaultBinding(), TrackServiceClient.GetDefaultEndpointAddress()) {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_ITrackService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public TrackServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(TrackServiceClient.GetBindingForEndpoint(endpointConfiguration), TrackServiceClient.GetEndpointAddress(endpointConfiguration)) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public TrackServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(TrackServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress)) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public TrackServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(TrackServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress) {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public TrackServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Threading.Tasks.Task<long> StartTrackingAsync(string deviceId, TrackViewer.Services.TrackService.TrackLocation location) {
            return base.Channel.StartTrackingAsync(deviceId, location);
        }
        
        public System.Threading.Tasks.Task PublishTrackingInfoAsync(long trackId, TrackViewer.Services.TrackService.TrackLocation location) {
            return base.Channel.PublishTrackingInfoAsync(trackId, location);
        }
        
        public System.Threading.Tasks.Task<long> StopTrackingAsync(long trackId) {
            return base.Channel.StopTrackingAsync(trackId);
        }
        
        public System.Threading.Tasks.Task<TrackViewer.Services.TrackService.TrackLocation> GetTrackingInfoAsync(long trackId) {
            return base.Channel.GetTrackingInfoAsync(trackId);
        }
        
        public System.Threading.Tasks.Task<long> StartTrackingRestfulAsync(string deviceId, string latitude, string longitude, string trackNo) {
            return base.Channel.StartTrackingRestfulAsync(deviceId, latitude, longitude, trackNo);
        }
        
        public System.Threading.Tasks.Task PublishTrackingInfoRestfulAsync(string latitude, string longitude, string trackNo) {
            return base.Channel.PublishTrackingInfoRestfulAsync(latitude, longitude, trackNo);
        }
        
        public System.Threading.Tasks.Task<TrackViewer.Services.TrackService.TrackLocation> GetTrackingInfoRestfulAsync(string trackId) {
            return base.Channel.GetTrackingInfoRestfulAsync(trackId);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync() {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync() {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_ITrackService)) {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration) {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_ITrackService)) {
                return new System.ServiceModel.EndpointAddress("http://127.0.0.1:81/TrackService.svc");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding() {
            return TrackServiceClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_ITrackService);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress() {
            return TrackServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_ITrackService);
        }
        
        public enum EndpointConfiguration {
            
            BasicHttpBinding_ITrackService,
        }
    }
}
