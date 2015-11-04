using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.Common.Processing
{
    public class StaticOrderPartlistResolver : IOrderPartlistResolver
    {
        public ICollection<string> ResolvePartlist(IEnumerable<string> features)
        {
            return new List<string>()
            {
                "8T0823359",
                "8T8971849",
                "8K0863335B",
                "8T8833707F",
                "8K5877203C",
                "8K5853345D",
                "8K0857791A",
                "8K0907063DE",
                "8K5857807AJ",
                "8T0035412D",
                "8T8945097",
                "4G0906093H",
                "8K0857833C"
            };
        }
    }
}
