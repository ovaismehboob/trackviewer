using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TrackContext: DbContext
    {
        public TrackContext() : base("TrackViewerDB")
        {

        }

        public DbSet<TrackUsers> TrackUsers { set; get; }        
    }

    public class TrackUsers
    {
        public Int64 Id { set; get; }

        public String DeviceId { set; get; }

        public String Name { set; get; }
        public String Email { set; get; }

        public String ActivationCode { set; get; }

        public bool IsActivated { set; get; }

    }
}
