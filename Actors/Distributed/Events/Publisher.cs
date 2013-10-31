using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors.Events
{   
    class Publisher<T> : IDisposable
    {
        List<Subscriber<T>> subscribers = new List<Subscriber<T>>();
        public void Subscribe(Subscriber<T> subscriber)
        {
            lock (subscribers)
                subscribers.Add(subscriber);
        }
        public void Unsubscribe(Subscriber<T> subscriber)
        {
            lock (subscribers)
                subscribers.Remove(subscriber);
        }
        public void Fire(T data)
        {
            lock (subscribers)
                foreach (var s in subscribers)
                    s.HandleEvent(data);
        }

        public void Dispose()
        {
            lock (subscribers)
                foreach (var s in subscribers)
                    s.Close(this);
        }
    }

    class Subscriber<T>
    {
        public Subscriber(Action<T> callback)
        {
            HandleEvent = callback;
        }
        public Action<T> HandleEvent { get; private set; }
        List<Publisher<T>> publishers = new List<Publisher<T>>();
        public void OnSubscribed(Publisher<T> t)
        {
            lock (publishers)
                publishers.Add(t);
        }
        internal void Close(Publisher<T> t)
        {
            lock (publishers)
                publishers.Remove(t);
        }
        public void Dispose()
        {
            lock (publishers)
                foreach (var publisher in publishers)
                    publisher.Unsubscribe(this);
        }
    }
}
