using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace Cls.Extensions
{
    public static class KeyValuePair
    {
         public static KeyValuePair<TKey, TValue> New<TKey, TValue>(TKey key, TValue value){
            return new KeyValuePair<TKey,TValue>(key, value);
        }
    }

	public static class Extensions    
	{
		class ConnectState{
			public TcpClient Client;
			public int State;
			public const int Empty = 0;
			public const int Connected = 1;
			public const int TimedOut = 2;
		}
		public static string ToDelimitedString<T>(this IEnumerable<T> items, string separator = ", "){
			if(items == null) return "";
			items = items.Where(n=>n != null);
			return string.Join<T>(separator, items);
		}

		public static TimeoutTimer ToTimeout(this TimeSpan s){
			return new TimeoutTimer(s);
		}

		public static string FormatWith(this string s, params object[] args){
			return string.Format(s, args);
		}

		public static U[] ToArray<T,U>(this IEnumerable<T> i, Func<T, U> func){
			return i.Select(func).ToArray();
		}
       
		public static void Connect(this TcpClient client, string host, int port, TimeSpan timeout){
			var state = new ConnectState{Client = client};
			client.BeginConnect(host, port, EndConnect, state);
						
			var sw = Stopwatch.StartNew();
			while(state.State == 0 && sw.Elapsed < timeout)			
				Thread.Sleep(100);
			
			var value = Interlocked.CompareExchange(ref state.State, ConnectState.TimedOut, ConnectState.Empty);
			if(value != ConnectState.Connected)
				throw new Exception("Connection timed out");
		}

		static void EndConnect(IAsyncResult result){
			try{
				var state = (ConnectState)result.AsyncState;
				state.Client.EndConnect(result);
				var value = Interlocked.CompareExchange(ref state.State, ConnectState.Connected, ConnectState.Empty);
				if(value != ConnectState.Empty)
					state.Client.Close();
			}catch{}
		}

        public static ArraySegment<T> Slice<T>(this T[] array, int? offset = null, int? exclusiveEnd = null)
        {
            if (offset.HasValue)
                while (offset.Value < 0)
                    offset += array.Length;
            if (exclusiveEnd.HasValue)
                while (exclusiveEnd.Value < 0)
                    exclusiveEnd += array.Length;
            int start = offset ?? 0;
            int end = exclusiveEnd ?? array.Length;
            return new ArraySegment<T>(array, start, end - start);
        }

        public static T[] ToArray<T>(this ArraySegment<T> array)
        {
            var r = new T[array.Count];
            Array.Copy(array.Array, array.Offset, r, 0, array.Count);
            return r;
        }
        
        public static int BinarySearch<T, U>(this List<T> list, T item, Func<T, U> getValue)
        {
            return list.BinarySearch(item, new Comparer<T, U>(getValue));
        }

        public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> remove)
        {
            var items = list.Where(n=> !remove(n)).ToList();
            list.Clear();
            list.AddRange(items);
        }

        public static int IndexOf<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].Equals(item))
                    return i;
            return -1;
        }

        public static int IndexOf<T>(this T[] array, Func<T, bool> found)
        {
            for (int i = 0; i < array.Length; i++)
                if (found(array[i]))
                    return i;
            return -1;
        }
        public static U Coalesce<T, U>(this T item, Func<T, U> func)
        {
            if (item == null) return default(U);
            return func(item);
        }
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }
        public static bool IsMatch(this string s, string regex)
        {
            if (s == null || regex == null) return false;
            return Regex.IsMatch(s, regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        public static int Bound(this int i, int low, int high)
        {
            return Math.Min(high, Math.Max(i, low));
        }

        public static byte[] ComputeHash(this HashAlgorithm hash, string str)
        {
            return hash.ComputeHash(Encoding.UTF8.GetBytes(str));
        }
        public static string Remove(this string s, string r)
        {
            return s.Replace(r, "");
        }

        public static byte[] Write(this byte[] b, int x, int offset = 0)
        {
            b[offset] = (byte)x;
            b[offset + 1] = (byte)(x >> 8);
            b[offset + 2] = (byte)(x >> 16);
            b[offset + 3] = (byte)(x >> 24);
            return b;
        }

        public static byte[] Write(this byte[] b, byte[] c, int offset = 0)
        {
            Buffer.BlockCopy(c, 0, b, offset, c.Length);
            return b;
        }

		public static T As<T>(this object o){
			return (T)o;
		}

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

        public static void AddRange<T>(this ICollection<T> a, IEnumerable<T> items)
        {
            foreach (var c in items)
                a.Add(c);
        }

      
	}
}

