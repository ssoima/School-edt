using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pentlandfirth.Data.EF;

namespace NextLAP.IP1.Storage.EntityFramework
{
    public abstract class Ip1BaseRepository<T> : EntityFrameworkRepositoryBase<T, Ip1Context> where T : class
    {
        protected Ip1BaseRepository(Ip1Context context) : base(context, false)
        {
        }

        public IQueryable<T> Entities
        {
            get { return Set; }
        }
    }
}
