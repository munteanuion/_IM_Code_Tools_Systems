using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _Scripts._AbstractSystems.StateMachine
{
    public interface IStateMachine
    {
        List<IState> States { get; }
        IState CurrentState { get; }
        IState PreviousState { get; }

        bool TrySetNextState(bool loopNextState = true);
        UniTask SetState<T>(bool canSetSameState = false) where T : IState;
        void AddState<T>(T state) where T : IState;
        IState GetState<T>() where T : IState;
        void Update(float deltaTime);
    }
}