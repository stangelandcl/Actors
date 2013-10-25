using System;
using System.Collections.Generic;
using Actors.Tasks;
using System.Linq;

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
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key)
        {
            TValue v;
            d.TryGetValue(key, out v);            
            return v;
        }

        public static T FindOrDefault<T>(this IEnumerable<T> items, T other)
        {
            return items.FirstOrDefault(n => n.Equals(other));
        }

        public static void AddRange<T>(this HashSet<T> a, IEnumerable<T> items)
        {
            foreach (var c in items)
                a.Add(c);
        }

        public static void FireEvent<T>(this Action<T> e, T args)
        {
            if (e != null)
                e(args);
        }
        public static void FireEventAsync<T>(this Action<T> e, T args)
        {
            if (e != null)
                TaskEx.Run(() => e(args));
        }

        public static void FireEvent<T>(this EventHandler<T> e, object sender, T args) where T : EventArgs
        {
            if (e != null)
                e(sender, args);
        }
        public static void FireEventAsync<T>(this EventHandler<T> e, object sender, T args) where T : EventArgs
        {
            if (e != null)
                TaskEx.Run(() => e(sender, args));
        }
       
	}
}

