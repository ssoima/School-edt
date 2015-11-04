using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = NextLAP.IP1.Models.Planning.Task;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class TaskRepository : Ip1BaseRepository<Task>
    {
        public TaskRepository(Ip1Context context) : base(context)
        {
        }
    }
}
