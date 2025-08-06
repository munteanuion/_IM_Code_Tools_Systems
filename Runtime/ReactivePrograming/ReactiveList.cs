using System;
using System.Collections.Generic;

namespace ReactivePrograming
{
    public class ReactiveList<T> : IDisposable
    {
        private readonly List<T> _elements = new();
        public IReadOnlyList<T> Elements => _elements;

        private readonly List<Subscriber<T>> _onAddSubscribers = new();
        private readonly List<Subscriber<T>> _toAddAddSubscribers = new();
        private readonly List<Subscriber<T>> _toRemoveAddSubscribers = new();

        private readonly List<Subscriber<T>> _onRemoveSubscribers = new();
        private readonly List<Subscriber<T>> _toAddRemoveSubscribers = new();
        private readonly List<Subscriber<T>> _toRemoveRemoveSubscribers = new();

        
        
        public virtual void Add(T element)
        {
            _elements.Add(element);
            Invoke(_onAddSubscribers, _toAddAddSubscribers, _toRemoveAddSubscribers, element);
        }

        public virtual void Remove(T element)
        {
            if (_elements.Remove(element))
                Invoke(_onRemoveSubscribers, _toAddRemoveSubscribers, _toRemoveRemoveSubscribers, element);
        }

        public IDisposable SubscribeOnAdd(Action<T> action, bool invokeWithAllExisting = false)
        {
            var subscriber = new Subscriber<T>(action, s => _toRemoveAddSubscribers.Add(s));
            _toAddAddSubscribers.Add(subscriber);

            if (invokeWithAllExisting)
            {
                foreach (var item in _elements)
                    action(item);
            }

            return subscriber;
        }

        public IDisposable SubscribeOnRemove(Action<T> action)
        {
            var subscriber = new Subscriber<T>(action, s => _toRemoveRemoveSubscribers.Add(s));
            _toAddRemoveSubscribers.Add(subscriber);
            return subscriber;
        }

        public void Unsubscribe(IDisposable subscription) => subscription.Dispose();

        public void UnsubscribeAll()
        {
            FlushSubscribers();
            for (int i = _onAddSubscribers.Count - 1; i >= 0; i--)
                _onAddSubscribers[i].Dispose();
            for (int i = _onRemoveSubscribers.Count - 1; i >= 0; i--)
                _onRemoveSubscribers[i].Dispose();
            FlushSubscribers();
        }

        public void Dispose() => UnsubscribeAll();



        private void Invoke(
            List<Subscriber<T>> subscribers,
            List<Subscriber<T>> toAdd,
            List<Subscriber<T>> toRemove,
            T element)
        {
            Flush(subscribers, toAdd, toRemove);

            foreach (var subscriber in subscribers)
                subscriber.Invoke(element);
        }

        private void FlushSubscribers()
        {
            Flush(_onAddSubscribers, _toAddAddSubscribers, _toRemoveAddSubscribers);
            Flush(_onRemoveSubscribers, _toAddRemoveSubscribers, _toRemoveRemoveSubscribers);
        }

        private void Flush(List<Subscriber<T>> main, List<Subscriber<T>> toAdd, List<Subscriber<T>> toRemove)
        {
            if (toAdd.Count > 0)
            {
                main.AddRange(toAdd);
                toAdd.Clear();
            }

            if (toRemove.Count > 0)
            {
                foreach (var subscriber in toRemove)
                    main.Remove(subscriber);
                toRemove.Clear();
            }
        }
    }
}
