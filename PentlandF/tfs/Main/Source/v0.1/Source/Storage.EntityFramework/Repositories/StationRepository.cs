using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Structure;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class StationRepository : Ip1BaseRepository<NextLAP.IP1.Models.Structure.Station>
    {
        public StationRepository(Ip1Context context) : base(context)
        {
        }
    }
}
