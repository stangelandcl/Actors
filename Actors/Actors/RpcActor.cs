﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace Cls.Actors
{
    public class RpcActor : Actor<IRpcMail>
    {
        public RpcActor()
        {            
            functions.Add(this, n => n.Name != "HandleMessage");           
        }

        protected RpcFunctions functions = new RpcFunctions();
                       
        protected override void HandleMessage(IRpcMail mail)
        {
            functions.Call(mail);                  
        }     
    }
}
