using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class Repository : IRepository
    {

        TrackContext Context;

        public Repository()
        {
            //Database.SetInitializer<EBRPContext>(new DropCreateDatabaseIfModelChanges<EBRPContext>());
            Context = new TrackContext();

        }

        public TrackContext GetContext
        {
            get { return Context; }
        }

        public Repository(TrackContext context)
        {
            Context = context;

        }

        public void CommitChanges()
        {
            Context.SaveChanges();
        }

        public T Single<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return All<T>().FirstOrDefault(expression);
        }

        public IQueryable<T> All<T>() where T : class
        {
            return Context.Set<T>().AsQueryable();
        }

        public virtual IQueryable<T> Filter<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Context.Set<T>().Where<T>(predicate).AsQueryable<T>();
        }

        public virtual IQueryable<T> Filter<T>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50) where T : class
        {
            int skipCount = index * size;
            var _resetSet = filter != null ? Context.Set<T>().Where<T>(filter).AsQueryable() : Context.Set<T>().AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public virtual T Create<T>(T TObject) where T : class
        {

            var newEntry = Context.Set<T>().Add(TObject);
            Context.SaveChanges();
            return newEntry;
        }

        public virtual int Delete<T>(T TObject) where T : class
        {
            Context.Set<T>().Remove(TObject);
            return Context.SaveChanges();
        }

        public virtual int Update<T>(T TObject) where T : class
        {
            try
            {
                var entry = Context.Entry(TObject);
                Context.Set<T>().Attach(TObject);
                entry.State = EntityState.Modified;
                return Context.SaveChanges();
            }
            catch (OptimisticConcurrencyException ex)
            {
                throw ex;
            }
        }

        public virtual int Delete<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var objects = Filter<T>(predicate);
            foreach (var obj in objects)
                Context.Set<T>().Remove(obj);
            return Context.SaveChanges();
        }

        public bool Contains<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Context.Set<T>().Count<T>(predicate) > 0;
        }

        public virtual T Find<T>(params object[] keys) where T : class
        {
            return (T)Context.Set<T>().Find(keys);
        }

        public virtual T Find<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Context.Set<T>().FirstOrDefault<T>(predicate);
        }


        public virtual void ExecuteProcedure(String procedureCommand, params SqlParameter[] sqlParams)
        {
            Context.Database.ExecuteSqlCommand(procedureCommand, sqlParams);

        }


        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            if (Context != null)
                Context.Dispose();
        }




        public void InitializeDatabase()
        {
            Context.Database.Initialize(true);
        }
    }
}
