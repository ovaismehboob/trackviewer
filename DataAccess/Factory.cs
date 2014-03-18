using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public abstract class Factory
    {
        public abstract IRepository FactoryMethod();
             
    }

    public class RepositoryInitiator : Factory
    {
        public override IRepository FactoryMethod()
        {
            return new Repository.Repository();
        }
    }
}
