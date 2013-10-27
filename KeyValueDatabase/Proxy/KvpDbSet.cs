using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

namespace KeyValueDatabase.Proxy
{
    public class KvpDbSetProxy<T> : IKvpDbSet<T>
    {
        public KvpDbSetProxy(IKvpDb db, string name)
        {
            this.db = db;
            this.name = name;
        }

        IKvpDb db;
        string name;

        public void Add(T item)
        {
            AddRange(Enumerable.Repeat(item, 1));
        }

        public void Remove(T item)
        {
            RemoveRange(Enumerable.Repeat(item, 1));
        }

        public T Get(T item)
        {
            return GetRange(Enumerable.Repeat(item, 1)).FirstOrDefault();
        }


        public void AddRange(IEnumerable<T> items)
        {
            lock (db)
            {
                var list = GetList();
                list.AddRange(items);
                db.Add(name, list);
            }
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            lock (db)
            {
                var list = GetList();
                foreach (var item in items)
                    list.Remove(item);
                db.Add(name, list);
            }
        }

        public IEnumerable<T> GetRange(IEnumerable<T> items)
        {
            lock (db)
            {
                var list = GetList();
                foreach (var item in items)
                {
                    int i = list.IndexOf(item);
                    if (i >= 0) yield return list[i];
                }
            }
        }
        public bool Any()
        {
            lock (db)
            {
                var list = GetList();
                return list.Any();
            }
        }

        private List<T> GetList()
        {
            return db.Get<List<T>>(name) ?? new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var list = GetList();
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
