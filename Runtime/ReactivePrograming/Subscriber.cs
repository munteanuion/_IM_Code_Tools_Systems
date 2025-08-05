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
        
        
        
        public void Dispose()
        {
            _onDispose?.Invoke(this);
            _action = null;
            _onDispose = null;
        }


        public void Invoke(T arg1, K arg2) => _action?.Invoke(arg1, arg2);
    }
}