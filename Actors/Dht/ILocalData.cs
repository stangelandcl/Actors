﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    public interface ILocalData 
    {
        KeyValuePair<string, byte[]>[] Data { get; }
        void AddRange(IEnumerable<KeyValuePair<string, byte[]>> items);

        byte[] Get(string key);
        void Add(string key, byte[] value);
        void Remove(string key);
    }
}
