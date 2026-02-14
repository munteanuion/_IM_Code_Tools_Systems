namespace _Scripts._AbstractSystems.StateMachine
{
    public abstract class State : IState
    {
        public abstract UniTaskVoid Enter();

        public abstract void Update(float deltaTime);

        public abstract UniTaskVoid Exit();
    }
}