using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    public abstract class MailActor<T>: Actor<IMail<T>>
    {
    }
    public abstract class MailActor : Actor<IMail> { }
}
