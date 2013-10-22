using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Actors
{
    public class ConvertEx
    {
        public static object ChangeType(object o, Type t)
        {
            if (o != null && o.GetType() == t)
                return o;
            
            var c = TypeDescriptor.GetConverter(t);
            if (o != null && c.CanConvertFrom(o.GetType()))
                return c.ConvertFrom(o);
            if (o != null)
            {
                c = TypeDescriptor.GetConverter(o.GetType());
                if (c.CanConvertTo(t))
                    return c.ConvertTo(o, t);
            }
            if (t.IsPrimitive)
                return Convert.ChangeType(o, t);
            if (o == null && t.IsClass)
                return true;
            if (t.IsAssignableFrom(o.GetType()))
                return o;
            throw new Exception("Cannot convert to " + t.Name + " from " +
                (o == null ? "(null)" : o.GetType().Name + ":" + o));
        }
    }
}
