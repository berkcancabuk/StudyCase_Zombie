using Managers;
using States.Interface;
using UnityEngine;

namespace States
{
    public class GameOverState : IGameState
    {
        private GameManager _gameManager;

        public GameOverState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Enter()
        {
            // UI'ı göster
            UIManager.Instance.ShowGameOver();
        
            // Spawn Manager'ı durdur
            _gameManager.spawnManager.StopSpawning();
        
            // Aktif düşmanları temizle
            _gameManager.spawnManager.DeactivateAllEnemies();
        
            // Player controller'ı devre dışı bırak
            if (_gameManager.playerController != null)
            {
                _gameManager.playerController.enabled = false;
            }
        
            // Zamanı durdur
            Time.timeScale = 0;
        }

        public void Exit()
        {
            Time.timeScale = 1;
        }

        public void Update() { }
    }
}