using System;

namespace ReactivePrograming
{
    public class Subscriber<T,K> : IDisposable
    {
        private Action<T, K> _action;
        private Action<Subscriber<T,K>> _onDispose;
        

        public Subscriber(Action<T, K> action, Action<Subscriber<T,K>> onDispose)
        {
            _action = action;
            _onDispose = onDispose;
        }
        
        
        public void Invoke(T arg1, K arg2) => _action?.Invoke(arg1, arg2);
        public void Dispose()
        {
            _onDispose?.Invoke(this);
            _action = null;
            _onDispose = null;
        }
    }

    
    
    public class Subscriber<T> : IDisposable
    {
        private Action<T> _action;
        private Action<Subscriber<T>> _onDispose;

        
        public Subscriber(Action<T> action, Action<Subscriber<T>> onDispose)
        {
            _action = action;
            _onDispose = onDispose;
        }

        
        public void Invoke(T value) => _action?.Invoke(value);
        public void Dispose()
        {
            _onDispose?.Invoke(this);
            _action = null;
            _onDispose = null;
        }
    }
}
