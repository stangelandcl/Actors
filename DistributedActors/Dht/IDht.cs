using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors.Dht
{
    public interface IDht : IDisposable
    {
        Task<T> Get<T>(string key);
        Task<IDht> Add<T>(string key, T value);
        Task<IDht> Remove(string key);

		void Subscribe(DhtOperation operation, string keyRegex);
		void Unsubscribe(DhtOperation operation, string regex);
		event Action<DhtOperation, string> KeyMatch;

        void Join(params ActorId[] others);
    }
}
