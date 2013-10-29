using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    public interface IMail<T>
    {
        IActorId From { get; }
        IActorId To { get; }
        T Message { get; set; }
    }
    public interface IMail : IMail<object[]>
    {
    }
    public struct FunctionCall{
        public string Name;
        public object[] Args;
    }
    public interface IFunctionCallMail : IMail<FunctionCall> { }
}
