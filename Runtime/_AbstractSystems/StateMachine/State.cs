namespace _Scripts._AbstractSystems.StateMachine
{
    public abstract class State : IState
    {
        public abstract void Enter();

        public abstract void Update(float deltaTime);

        public abstract void Exit();
    }
}