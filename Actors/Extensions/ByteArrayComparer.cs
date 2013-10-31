using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Extensions
{
    public class ByteArrayComparer : IComparer<byte[]>, IEqualityComparer<byte[]>
    {
        public static readonly ByteArrayComparer Default = new ByteArrayComparer();
        public int Compare(byte[] x, byte[] y)
        {
            for (int i = 0; i < x.Length && i < y.Length; i++)
            {
                int c = (int)x[i] - y[i];
                if (c != 0) return c;
            }
            return x.Length - y.Length;
        }       

        public bool Equals(byte[] x, byte[] y)
        {
            if (x.Length != y.Length) return false;
            for (int i = 0; i < x.Length; i++)
                if (x[i] != y[i])
                    return false;
            return true;
        }

        public int GetHashCode(byte[] obj)
        {
            int b = 37;
            for (int i = 0; i < obj.Length; i++)
            {
                b *= 37;
                b += obj[i];
            }
            return b;
        }
    }

  
    class Comparer<T,U> : IComparer<T>
    {
        public Comparer(Func<T,U> getValue){
            this.getValue = getValue;
            if (typeof(U) == typeof(byte[]))
                this.comparer =(IComparer<U>) ByteArrayComparer.Default;
            else
                this.comparer = Comparer<U>.Default;
        }
        Func<T, U> getValue;
        IComparer<U> comparer;
       

        public int Compare(T x, T y)
        {
            return comparer.Compare(getValue(x), getValue(y));
        }
    }
}
