using System;
using System.Collections.Generic;

namespace Actors
{
	public static class Extensions
	{
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key){
			TValue v;
			if(!d.TryGetValue(key, out v))
				d[key] = v = Activator.CreateInstance<TValue>();
			return v;
		}

        public static void AddRange<T>(this HashSet<T> a, IEnumerable<T> items)
        {
            foreach (var c in items)
                a.Add(c);
        }
	}
}

