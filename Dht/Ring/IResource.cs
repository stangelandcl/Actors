using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dht.Ring
{
    public interface IResource : IEquatable<IResource>
    {
    }

	public class StringResource : IResource{
		public StringResource(string s){
			this.Text = s;
		}
		public string Text {get; set;}

		public override int GetHashCode ()
		{
			return Text.GetHashCode();
		}

		public override bool Equals (object obj)
		{
			return Equals(obj as IResource);
		}

		#region IEquatable implementation
		public bool Equals (IResource other)
		{
			if(other == null || other.GetType() != GetType())
				return false;
			return ((StringResource)other).Text == Text;
		}
		#endregion
	}
}
