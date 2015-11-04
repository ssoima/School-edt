using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Structure;
using Pentlandfirth.Data.EF;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class CompanyRepository : Ip1BaseRepository<Company>
    {
        public CompanyRepository(Ip1Context context) : base(context)
        {
        }
    }
}
