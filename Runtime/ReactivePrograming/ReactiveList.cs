using System;
using System.Collections.Generic;

namespace ReactivePrograming
{
    public class ReactiveList<T>
    {
        public event Action<T> OnAdded;
        public event Action<T> OnRemoved;
        public IReadOnlyList<T> Elements => _elements;
        
        private List<T> _elements = new();

        
        
        public virtual void Add(T element)
        {
            _elements.Add(element);
            OnAdded?.Invoke(element);
        }

        public virtual void Remove(T element)
        {
            _elements.Remove(element);
            OnRemoved?.Invoke(element);
        }
    }
}