using System;
using System.Collections.Generic;

namespace ReactivePrograming
{
    public class ReactiveVar<T>
    {
        private readonly List<Subscriber<T, T>> _subscribers = new();
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
                    OnChanged?.Invoke(oldValue, _value);
            }
        }
    }
}
