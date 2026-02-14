using Cysharp.Threading.Tasks;

namespace _Scripts._AbstractSystems.StateMachine
{
    public abstract class State : IState
    {
        public abstract UniTask Enter();

        public abstract void Update(float deltaTime);

        public abstract UniTask Exit();
    }
}