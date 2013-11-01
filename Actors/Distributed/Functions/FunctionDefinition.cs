using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Actors
{
    class FunctionDefinition
    {
        public string Name { get; set; }
        public Type[] Args { get; set; }

        public static implicit operator FunctionDefinition(MethodInfo m)
        {
            return new FunctionDefinition
            {
                Name = m.Name, 
                Args = m.GetParameters().Select(n=>n.ParameterType).ToArray(),
            };
        }        
    }
}
