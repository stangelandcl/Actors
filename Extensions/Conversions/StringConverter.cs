using System;
using System.ComponentModel;
using Fasterflect;

namespace Cls.Extensions
{
	public class StringConverter<T> : TypeConverter
	{
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if(string.IsNullOrEmpty(value as string)) return typeof(T).CreateInstance();
			if(value.GetType() == typeof(string))
				return typeof(T).CreateInstance(value);
			return null;
		}
	}
}

