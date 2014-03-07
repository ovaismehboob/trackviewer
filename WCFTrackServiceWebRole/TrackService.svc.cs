using DataAccess;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFTrackServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class TrackService : ITrackService
    {
        
        public long StartTracking(String deviceId, TrackLocation location)
        {
            Random random=new Random();
            int trackNo= random.Next(1000000,9999999);
            if (TrackStorage.userTracks[deviceId] == null)
            {
                location.TrackNo = trackNo;
                TrackStorage.userTracks.Add(deviceId, location);
            }
            else
            {
                var locationInstance = (TrackLocation)TrackStorage.userTracks[deviceId];
                trackNo = locationInstance.TrackNo;
                location.TrackNo = trackNo;
                TrackStorage.userTracks[deviceId] = location;
                
            }
            return trackNo;
        }

        public void PublishTrackingInfo(long trackId, TrackLocation location)
        {

            var enumerator= TrackStorage.userTracks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var val = (TrackLocation) enumerator.Value;
                if (val.TrackNo == trackId) { 
                    val = location;
                    break;
                }
            }
            //TrackStorage.userTracks[trackId] = location;\
        }

        public long StopTracking(long trackId)
        {
            TrackStorage.userTracks.Remove(trackId);
            return 100;
        }


        public TrackLocation GetTrackingInfo(long trackId)
        {
           // return ((TrackStorage.userTracks[trackId] == null) ? null : (TrackLocation)TrackStorage.userTracks[trackId]);

            var enumerator = TrackStorage.userTracks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var val = (TrackLocation)enumerator.Value;
                if (val.TrackNo == trackId)
                    return val;
            } return null;
        }


        public long StartTrackingRestful(String deviceId, String latitude, String longitude, String trackNo)
        {
            return StartTracking(deviceId, new TrackLocation { Latitude = Convert.ToDouble(latitude), Longitude = Convert.ToDouble(longitude), TrackNo = Convert.ToInt32(trackNo) });
        }


        public void PublishTrackingInfoRestful(string latitude, string longitude, string trackNo)
        {
            PublishTrackingInfo(Convert.ToInt64(trackNo), new TrackLocation { TrackNo = Convert.ToInt32(trackNo), Longitude = Convert.ToDouble(longitude), Latitude = Convert.ToDouble(latitude) });
        }

        public TrackLocation GetTrackingInfoRestful(string trackId)
        {
            return GetTrackingInfo(Convert.ToInt64(trackId));
        }


        public bool IsUserRegistered(string deviceId)
        {
            try
            {
                IRepository repo = new RepositoryInitiator().FactoryMethod();
                var results = repo.All<TrackUsers>().Where(i => i.DeviceId == deviceId).ToList();
                if (results != null && results.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception) { return false; }
        }

        public bool IsUserActivated(string deviceId)
        {
            try
            {
                IRepository repo = new RepositoryInitiator().FactoryMethod();
                var results = repo.All<TrackUsers>().Where(i => i.DeviceId == deviceId && i.IsActivated==true).ToList();
                if (results != null && results.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception) { return false; }

        }

        public long RegisterUser(string deviceId, string activationCode, string name, string emailAddress)
        {
            try
            {
                IRepository repo = new RepositoryInitiator().FactoryMethod();
                Random random = new Random();
                activationCode = random.Next(1000, 9999).ToString();
                var result = repo.Create<TrackUsers>(new TrackUsers { DeviceId = deviceId, ActivationCode = activationCode, Email = emailAddress, Name = name });
                if (result != null)
                {
                    EmailManager.GetInstance().SendEmail(name, emailAddress, activationCode);
                    return result.Id;
                }
                else return -1;
            }
            catch (Exception) { return -2; }
        }

        public bool UpdateIsActivated(string deviceId, string activationCode)
        {
            try
            {
                IRepository repo = new RepositoryInitiator().FactoryMethod();
                var result = repo.All<TrackUsers>().Where(i => i.DeviceId == deviceId && i.ActivationCode==activationCode).First();
                if (result != null)
                {
                    result.IsActivated = true;
                    repo.Update<TrackUsers>(result);
                    EmailManager.GetInstance().DevelopCompletionEmailMessage(result.Name);
                    return true;
                }
                else { return false; }
            }
            catch (Exception) { return false; }
        }


        public void ResendCode(string deviceId, string emailAddress)
        {
            IRepository repo = new RepositoryInitiator().FactoryMethod();
            var user= repo.All<TrackUsers>().Where(i => i.DeviceId == deviceId).First();
            user.Email = emailAddress;
            repo.Update(user);
            EmailManager.GetInstance().SendEmail(user.Name, user.Email, user.ActivationCode);
        }


        public TrackViewerUser GetUserInfo(string deviceId)
        {
            IRepository repo = new RepositoryInitiator().FactoryMethod();
            var user = repo.All<TrackUsers>().Where(i => i.DeviceId == deviceId).First();
            if (user != null)
            {
                return new TrackViewerUser { Name = user.Name, ActivationCode = user.ActivationCode, DeviceId = user.DeviceId, Email = user.Email };
            }
            else return new TrackViewerUser();
        }
    }
}
