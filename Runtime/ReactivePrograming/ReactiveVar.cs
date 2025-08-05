using System;
using System.Collections.Generic;

namespace ReactivePrograming
{
    public class ReactiveVar<T> : IReadOnlyVar<T>, IDisposable
    {
        private readonly List<Subscriber<T, T>> _subscribers = new();
        private readonly List<Subscriber<T, T>> _toAddSubscribers = new();
        private readonly List<Subscriber<T, T>> _toRemoveSubscribers = new();
        
        private readonly IEqualityComparer<T> _comparer;
        private T _value;

        
        
        public ReactiveVar() : this(default(T)) { }
        
        public ReactiveVar(T value) : this(value, EqualityComparer<T>.Default) { }

        private ReactiveVar(T value, IEqualityComparer<T> comparer) 
        {
            _value = value;
            _comparer = comparer;
        }

        
        
        public T Value
        {
            get => _value;
            set
            {
                T oldValue = _value;

                _value = value;

                if (_comparer.Equals(_value,oldValue) == false)
                    Invoke(oldValue, _value);
            }
        }

        
        
        public IDisposable Subscribe(Action<T,T> action, bool invokeOnSubscribe = true)
        {
            Subscriber<T, T> subscriber = new Subscriber<T, T>(action, Remove);
            _toAddSubscribers.Add(subscriber);
            
            if (invokeOnSubscribe) subscriber.Invoke(_value, _value);
            
            return subscriber;
        }
        
        public void UnSubscribeAll()
        {
            SortSubscribersLists();
            
            for (int i = _subscribers.Count - 1; i >= 0; i--)
                _subscribers[i].Dispose();
            
            SortSubscribersLists();
        }
        
        public void Dispose() => UnSubscribeAll();
        public void UnSubscribe(IDisposable subscription) => subscription.Dispose();
        private void Remove(Subscriber<T, T> subscriber) => _toRemoveSubscribers.Add(subscriber);
        
        
        
        private void Invoke(T oldValue, T newValue)
        {
            SortSubscribersLists();

            foreach (Subscriber<T,T> subscriber in _subscribers)
                subscriber.Invoke(oldValue, newValue);
        }

        
        private void SortSubscribersLists()
        {
            if (_toAddSubscribers.Count > 0)
            {
                _subscribers.AddRange(_toAddSubscribers);
                _toAddSubscribers.Clear();
            }

            if (_toRemoveSubscribers.Count > 0)
            {
                foreach (Subscriber<T,T> subscriber in _toRemoveSubscribers)
                    _subscribers.Remove(subscriber);
                
                _toRemoveSubscribers.Clear();
            }
        }
    }
}
