using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.Common.Processing
{
    public interface IOrderPartlistResolver
    {
        ICollection<string> ResolvePartlist(IEnumerable<string> features);
    }
}
