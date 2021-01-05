using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend
{
    public  class ConectionMapping
    {
        public static ConectionMapping Instance { get; protected set; } = new ConectionMapping();
        private  readonly Dictionary<long, HashSet<string>> _connections = new Dictionary<long, HashSet<string>>();

        public  int Count()
        {        
            return _connections.Count;
        }

        public void Add(long key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }
        public void Remove(long key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
        public HashSet<string> Find(long key)
        {
            lock (_connections)
            {
                HashSet<string> conectionIds;
                _connections.TryGetValue(key, out conectionIds);
                return conectionIds;
            }
         
            
        }
    }
}
