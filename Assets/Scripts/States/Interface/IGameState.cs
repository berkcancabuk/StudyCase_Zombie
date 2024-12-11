namespace States.Interface
{
    public interface IGameState
    {
        void Enter();
        void Update();
        void Exit();
    }
}