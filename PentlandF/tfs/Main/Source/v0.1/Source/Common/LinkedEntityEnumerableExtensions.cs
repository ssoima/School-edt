using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.Common
{
    public static class LinkedEntityEnumerableExtensions
    {
        public static List<TLinkedEntity> ToListOrderedBySequence<TLinkedEntity>(this IEnumerable<TLinkedEntity> source) where TLinkedEntity : class, ILinkedEntity
        {
            var list = source.ToList();
            if (list.Count < 2) return list;
            var result = new List<TLinkedEntity>(list.Count);
            var current = list.FirstOrDefault(x => x.PredecessorId == null);
            if (current == null)
                throw new InvalidOperationException("The sequence does not contain an item with PredecessorId == null");
            result.Add(current);
            while (list.Count != result.Count)
            {
                var c = current;
                current = list.Find(x => x.PredecessorId == c.Id);
                if (current == null) throw new InvalidOperationException("There is an error in the sequence");
                result.Add(current);
            }
            return result;
        } 
    }
}
