using Managers;
using States.Interface;
using UnityEngine;

namespace States
{
    public class LevelTransitionState : IGameState
    {
        private readonly GameManager _gameManager;

        public LevelTransitionState(GameManager gm)
        {
            _gameManager = gm;
        }

        public void Enter()
        {
            Debug.Log("LevelTransition State: Enter");
            // Yeni seviye verilerini yükle, sahneyi değiştir veya mevcut seviyeyi resetle.
            // Örneğin sahne yükleme asenkron başlayabilir.
        }

        public void Update()
        {
            // Sahne yüklendiğinde veya seviye verileri hazır olduğunda:
            // if (_gameManager.IsNextLevelReady())
            // {
            //     _gameManager.StateMachine.ChangeState(new PlayingState(_gameManager, _gameManager.GetCurrentLevelDuration()));
            // }
        }

        public void Exit()
        {
            Debug.Log("LevelTransition State: Exit");
            // Gerekirse yükleme ekranını kapat.
        }
    }
}