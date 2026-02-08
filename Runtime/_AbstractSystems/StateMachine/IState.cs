namespace _Scripts._AbstractSystems.StateMachine
{
    public interface IState
    {
        public void Enter();
        public void Update(float deltaTime);
        public void Exit();
    }
}