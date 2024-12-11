using States.Interface;

namespace States
{
    public class GameStateMachine
    {
        private IGameState _currentState;

        public void ChangeState(IGameState newState)
        {
            // Eğer mevcut bir durum varsa önce Exit çağır
            if (_currentState != null)
                _currentState.Exit();

            // Yeni duruma geç
            _currentState = newState;

            // Yeni durumun Enter metodu çağır
            _currentState.Enter();
        }

        public void Update()
        {
            // Mevcut durumun update fonksiyonunu her frame çağır
            if (_currentState != null)
                _currentState.Update();
        }
    }
}