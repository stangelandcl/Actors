using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Actors
{
    public interface IKvpDbSet<T> : IEnumerable<T>
    {
        void AddRange(IEnumerable<T> item);
        void RemoveRange(IEnumerable<T> item);
        IEnumerable<T> GetRange(IEnumerable<T> items);
        void Add(T item);
        void Remove(T item);
        T Get(T item);
        bool Any();
    }
}
