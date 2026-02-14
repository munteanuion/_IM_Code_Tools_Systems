using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Scripts._AbstractSystems.StateMachine
{
    public class StateMachine : IStateMachine
    {
        public IState CurrentState { get; protected set; }
        public IState PreviousState { get; protected set; }
        public List<IState> States { get; protected set; } = new();


        public virtual bool TrySetNextState(bool loopNextState = true)
        {
            if (States.Count == 0)
            {
                Debug.Log("No states available. Cannot set next state.");
                return false;
            }

            if (CurrentState == null)
            {
                CurrentState = States[0];
                CurrentState.Enter().Forget();
                return true;
            }

            var nextState = GetNextState(loopNextState);
            if (nextState == null)
                return false;

            if (nextState == CurrentState)
                return false;

            CurrentState.Exit().Forget();
            PreviousState = CurrentState;
            CurrentState = nextState;
            CurrentState.Enter().Forget();
            return true;
        }

        public async virtual UniTask SetState<T>(bool canSetSameState = false) where T : IState
        {
            var state = GetState<T>();
            if (state == null || (!canSetSameState && state == CurrentState))
            {
                Debug.Log("State is null");
                return;
            }

            if (CurrentState != null) 
                await CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = state;
            await CurrentState.Enter();
        }

        public virtual void AddState<T>(T state) where T : IState
        {
            if (state == null)
                return;

            foreach (var existingState in States)
            {
                if (existingState is T)
                    return;
            }

            States.Add(state);
        }

        public virtual IState GetState<T>() where T : IState
        {
            foreach (var state in States)
                if (state is T targetState) return targetState;

            return null;
        }

        public virtual void Update(float deltaTime)
        {
            if (CurrentState != null)
                CurrentState.Update(deltaTime);
        }
        
        
        
        private IState GetNextState(bool loopNextState = true)
        {
            if (States.Count == 0)
                return null;

            if (CurrentState == null)  
                return States.Count > 0 ? States[0] : null;

            int currentIndex = States.IndexOf(CurrentState);
            if (currentIndex == -1)
                return States[0]; 

            int nextIndex = currentIndex + 1;

            if (nextIndex >= States.Count)
            {
                if (loopNextState)
                    return States[0]; 
                else
                    return null; 
            }

            return States[nextIndex];
        }
    }
}
