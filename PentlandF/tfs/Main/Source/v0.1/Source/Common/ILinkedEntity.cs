using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.Common
{
    public interface ILinkedEntity
    {
        long Id { get; set; }
        long? PredecessorId { get; set; }
    }

    public interface ILinkedEntity<T> : ILinkedEntity
    {
        T Predecessor { get; set; }
    }
}
