using Data;
using Managers;
using States.Interface;
using UnityEngine;

namespace States
{
    public class PlayingState : IGameState
    {
        private GameManager _gameManager;
        private UIManager _uiManager;
        private float _levelDuration;
        private LevelData _levelData;
        private float _remainingTime;

        public PlayingState(GameManager gameManager, float levelDuration, UIManager uiManager, LevelData levelData)
        {
            _gameManager = gameManager;
            _levelDuration = levelDuration;
            _uiManager = uiManager;
            _levelData = levelData;
            _remainingTime = levelDuration;
        }

        public void Enter()
        {
            _uiManager.HideMainMenu();
            _uiManager.HideGameWon();
        
            EnablePlayerController();
        
            _gameManager.spawnManager.SetLevelParameters(_levelData);
        
            _gameManager.spawnManager.StartSpawning();
            AudioManager.Instance.Play(SoundType.BackgroundMusic);
        
            Time.timeScale = 1;
        }

        public void Update()
        {
            _remainingTime -= Time.deltaTime;
            _uiManager.UpdateTimer(_remainingTime);

            if (_remainingTime <= 0)
            {
                _gameManager.StateMachine.ChangeState(new GameOverState(_gameManager));
            }
        }

        private void EnablePlayerController()
        {
            if (_gameManager.playerController != null)
            {
                _gameManager.playerController.enabled = true;
                _gameManager.playerController.ResetPlayer();
            }
        }

        public void Exit()
        {
            _gameManager.spawnManager.StopSpawning();
        }
    }
}