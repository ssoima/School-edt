using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Structure;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class SectionRepository : Ip1BaseRepository<Section>
    {
        public SectionRepository(Ip1Context context) : base(context)
        {
        }
    }
}
