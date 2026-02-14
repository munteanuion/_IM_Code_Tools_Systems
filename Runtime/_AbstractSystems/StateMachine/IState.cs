using Cysharp.Threading.Tasks;

namespace _Scripts._AbstractSystems.StateMachine
{
    public interface IState
    {
        public UniTask Enter();
        public void Update(float deltaTime);
        public UniTask Exit();
    }
}