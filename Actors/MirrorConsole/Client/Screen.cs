using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Actors
{
    public class Screen
    {
        public Screen(int x, int y)
        {
            Width = x;
            Height = y; 
            Buffer = new char[y * x];
            Attributes = new Attributes[Buffer.Length];
        }

        public int Width {get;set;}
        public int Height {get;set;}
        public char[] Buffer {get;set;}
        public Attributes[] Attributes { get; set; }

        public void SetAttribute(int x, int y, Attributes z)
        {
            Attributes[Offset(x, y)] = z;
        }
        public Attributes GetAttribute(int x, int y)
        {
            return Attributes[Offset(x, y)];
        }

        private int Offset(int x, int y)
        {
            return y * Width + x;
        }
        public char this[int x, int y]
        {
            get { return Buffer[Offset(x,y)]; }
            set { Buffer[Offset(x,y)] = value; }
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() * 37 ^ Height.GetHashCode() + Buffer[0].GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var x = (Screen)obj;
            if (x.Height != Height || x.Width != Width || 
                x.Buffer.Length != Buffer.Length || x.Attributes.Length != Attributes.Length)
                return false;

            for (int i = 0; i < Buffer.Length; i++)
                if (Buffer[i] != x.Buffer[i] || Attributes[i] != x.Attributes[i])
                    return false;
            return true;
        }
    }
}
