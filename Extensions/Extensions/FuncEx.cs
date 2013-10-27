using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fasterflect;
namespace System
{
    class FuncEx
    {
        public static object NewFunc(Func<object> func, Type returnType)
        {
            var converter = typeof(DelegateConverter<>).MakeGenericType(returnType)
                .CreateInstance(func);
            var del = Delegate.CreateDelegate(typeof(Func<>).MakeGenericType(returnType),
                converter, converter.GetType().Method("Delegate"));
            return del;
        }
    }

    class DelegateConverter<T>
    {
        public DelegateConverter(Func<object> func)
        {
            this.func = func;
        }
        Func<object> func;
        public T Delegate()
        {
            return (T)func();
        }
    }

}
