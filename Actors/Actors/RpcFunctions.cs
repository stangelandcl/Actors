using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Fasterflect;
using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;


namespace Cls.Actors
{
    public class RpcFunctions
    {       
        Dictionary<string, Tuple<object, MethodInfo>> functions = new Dictionary<string, Tuple<object, MethodInfo>>();

        public void Add(object obj, Func<MethodInfo, bool> keep = null)
        {
            if (keep == null) keep = n => true;
            foreach (var method in GetMethodMatches(obj, keep))
            {
				functions[method.Name] = Tuple.Create(obj, method);
            }                 
        }

		public void Remove(string name){
			functions.Remove(name);
		}

        private IEnumerable<MethodInfo> GetMethodMatches(object obj, Func<MethodInfo, bool> keep)
        {
            return GetTypes(obj.GetType()).SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(n => n.GetParameters().Length > 0 && typeof(IMail).IsAssignableFrom(n.GetParameters()[0].ParameterType))
                             .Where(n => keep(n)));
        }

        IEnumerable<Type> GetTypes(Type t)
        {
			if(t == null || t == typeof(object))
				yield break;
            yield return t;           
            foreach (var x in GetTypes(t.BaseType))
                yield return x;
        }

        public void Call(IRpcMail mail)
        {
            var func = GetFunction(mail);
            if (func == null) return;

            var args = ConvertParams(mail, func.Item2);
            func.Item2.Call(func.Item1, args);
        }

        private Tuple<object, MethodInfo> GetFunction(IRpcMail mail)
        {
            Tuple<object, MethodInfo> func;
            if (!functions.TryGetValue(mail.Message.Name, out func) ||
                            mail.Message.Args.Length != func.Item2.GetParameters().Length - 1)
                return null;
            return func;
        }

        private static object[] ConvertParams(IRpcMail mail, MethodInfo func)
        {
            var p = func.Parameters();
            var args = new object[p.Count];
            args[0] = mail;
            for (int i = 0; i < mail.Message.Args.Length; i++)
                args[i + 1] = ConvertEx.Convert(mail.Message.Args[i], p[i + 1].ParameterType);
            return args;
        }
    }
}
