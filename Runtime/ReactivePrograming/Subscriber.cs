using System;

namespace ReactivePrograming
{
    public class Subscriber<T,K>
    {
        private Action<T, K> _action;



        public Subscriber(Action<T, K> action)
        {
            _action = action;
        }
        
        
        
        public void Invoke(T arg1, K arg2) => _action?.Invoke(arg1, arg2);
    }
}