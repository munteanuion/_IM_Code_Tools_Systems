using System.Collections.Generic;

namespace _Scripts._AbstractSystems.StateMachine
{
    public interface IStateMachine
    {
        List<IState> States { get; }
        IState CurrentState { get; }
        IState PreviousState { get; }

        bool TrySetNextState(bool loopNextState = true);
        UniTaskVoid SetState<T>(bool canSetSameState = false) where T : IState;
        void AddState<T>(T state) where T : IState;
        IState GetState<T>() where T : IState;
        void Update(float deltaTime);
    }
}