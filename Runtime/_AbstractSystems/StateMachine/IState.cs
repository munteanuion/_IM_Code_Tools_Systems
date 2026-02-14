namespace _Scripts._AbstractSystems.StateMachine
{
    public interface IState
    {
        public UniTaskVoid Enter();
        public void Update(float deltaTime);
        public UniTaskVoid Exit();
    }
}