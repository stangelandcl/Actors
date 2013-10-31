using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    public abstract class Actor<T> : MessageLoop<T>
    {           
        
    }

    public abstract class Actor : Actor<object> 
    {       
    }
}
