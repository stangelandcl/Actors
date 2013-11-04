using System;
using System.Collections.Generic;
using Cls.Extensions;

namespace Cls.Serialization
{
	class TwoWayDictionary<T1, T2>
	{
		Dictionary<T1, T2> m1 = new Dictionary<T1, T2>();
		Dictionary<T2, T1> m2 = new Dictionary<T2, T1>();
		

		public void Add(T1 a, T2 b){
			lock(m1)
				m1[a] = b;
			lock(m2)
				m2[b] = a;
		}

		public T2 Get(T1 a, Func<T1, T2> get){
			var b = Get(a);
			if(b.HasValue) return b.Value;
			lock(m1)
				return m1[a] = get(a);
		}

		public T1 Get(T2 a, Func<T2, T1> get){
			var b = Get(a);
			if(b.HasValue) return b.Value;
			lock(m2)
				return m2[a] = get(a);
		}

		public Option<T2> Get(T1 a){
			lock(m1)
			{
				T2 b;
				if(m1.TryGetValue(a, out b))
					return Option.Some(b);
				return Option.None<T2>();
			}
		}

		public Option<T1> Get(T2 a){
			lock(m1)
			{
				T1 b;
				if(m2.TryGetValue(a, out b))
					return Option.Some(b);
				return Option.None<T1>();
			}
		}

	}
}

