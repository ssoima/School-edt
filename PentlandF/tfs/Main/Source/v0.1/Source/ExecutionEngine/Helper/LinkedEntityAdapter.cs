using MongoRepository;
using NextLAP.IP1.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.ExecutionEngine.Helper
{
    public sealed class LinkedEntityAdapter<T> : ILinkedEntity 
    {
        private T _entity;
        private Func<T, long> _idSelector;
        private Func<T, long?> _predecessorSelector;
        public LinkedEntityAdapter(T entity, Func<T, long> idSelector, Func<T, long?> predecessorSelector)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (idSelector == null) throw new ArgumentNullException("idSelector");
            if (predecessorSelector == null) throw new ArgumentNullException("predecessorSelector");
            _entity = entity;
            _idSelector = idSelector;
            _predecessorSelector = predecessorSelector;
        }
        public long Id {
            get
            {
                return _idSelector(_entity);
            }
            set
            {
            }
        }
        public long? PredecessorId { get { return _predecessorSelector(_entity); } set { } }
        internal T Adaptee { get { return _entity; } }
    }
}
