using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;

namespace NextLAP.IP1.Models.Planning
{
    public class TaskAssignmentImage : BaseEntity
    {
        public string Hash { get; set; }
        public string Type { get; set; }
        public byte[] Image { get; set; }
    }
}
