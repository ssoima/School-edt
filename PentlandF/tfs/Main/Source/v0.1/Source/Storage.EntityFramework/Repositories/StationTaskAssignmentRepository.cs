using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Planning;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class StationTaskAssignmentRepository : Ip1BaseRepository<StationTaskAssignment>
    {
        public StationTaskAssignmentRepository(Ip1Context context) : base(context)
        {
        }
    }
}
