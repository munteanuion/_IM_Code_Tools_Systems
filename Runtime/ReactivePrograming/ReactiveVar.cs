using System;
using System.Collections.Generic;

namespace ReactivePrograming
{
    public class ReactiveVar<T> : IReadOnlyReactiveVar<T>
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

        
        public IDisposable Subscribe(Action<T,T> action)
        {
            Subscriber<T, T> subscriber = new Subscriber<T, T>(action, Remove);
            _toAddSubscribers.Add(subscriber);
            return subscriber;
        }

        public void Remove(Subscriber<T, T> subscriber) => _toRemoveSubscribers.Remove(subscriber);

        
        
        private void Invoke(T oldValue, T newValue)
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

            foreach (Subscriber<T,T> subscriber in _subscribers)
                subscriber.Invoke(oldValue, newValue);
        }
    }
}
