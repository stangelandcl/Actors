using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Actors
{
    public class RpcActor : Actor<IFunctionCallMail>
    {
        public RpcActor()
        {
            functions = GetTypes(GetType()).SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
              .Where(n => n.GetParameters().Length > 0 && n.GetParameters()[0].ParameterType == typeof(IFunctionCallMail))
              .Where(n => n.Name != "Execute"))
              .ToDictionary(n => n.Name);
        }
        Dictionary<string, MethodInfo> functions;
        IEnumerable<Type> GetTypes(Type t)
        {
            yield return t;
            if (typeof(RpcActor).IsAssignableFrom(t.BaseType))
                foreach (var x in GetTypes(t.BaseType))
                    yield return x;
        }
        protected override void HandleMessage(IFunctionCallMail mail)
        {
            try
            {
                MethodInfo func;
                if (!functions.TryGetValue(mail.Message.Name, out func) ||
                    mail.Message.Args.Length != func.GetParameters().Length - 1)
                    return;
                var p = func.GetParameters();
                var args = new object[p.Length];
                args[0] = mail;
                for (int i = 0; i < mail.Message.Args.Length; i++)
                    args[i + 1] = ConvertEx.Convert(mail.Message.Args[i], p[i + 1].ParameterType);
                Run(() => func.Invoke(this, args));
            }
            catch (Exception ex)
            {
                Die(ex.ToString());
            }
            
        }
    }
}
