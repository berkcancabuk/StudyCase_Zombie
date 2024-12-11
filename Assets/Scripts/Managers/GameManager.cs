using Controllers;
using Data;
using UnityEngine;
using States;
using UnityEngine.Serialization;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public GameStateMachine StateMachine { get; private set; }

        public SpawnManager spawnManager;
        [SerializeField] private float levelDuration = 5f;
        public PlayerController playerController;
        public LevelData[] levels;
        private int _currentLevelIndex;
        private int _currentLevel = 1;
        public int _defatedTotalEnemiesCount = 0;
        public int _sessionDefeatedEnemies = 0;
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            StateMachine = new GameStateMachine();
        }

        private void Start()
        {
            UIManager.Instance.Initialize(this);

            LoadDefeatedEnemies();

            StateMachine.ChangeState(new MainMenuState(this));
            //LoadCurrentLevel();
        }

        private void Update()
        {
            StateMachine.Update();
        }

        public void ShowMainMenuUI()
        {
            UIManager.Instance.ShowMainMenu();
        }

        public void HideMainMenuUI()
        {
            UIManager.Instance.HideMainMenu();
        }

        public void ShowGameWonUI()
        {
            UIManager.Instance.ShowGameWon(_sessionDefeatedEnemies,_defatedTotalEnemiesCount);
        }

        public void HideGameWonUI()
        {
            UIManager.Instance.HideGameWon();
        }
        

        // private void LoadCurrentLevel()
        // {
        //     _currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 0);
        // }

        private void SaveCurrentLevel()
        {
            PlayerPrefs.SetInt("CurrentLevel", _currentLevelIndex);
            PlayerPrefs.Save();
        }

        public void OnStartButtonClicked()
        {
            UIManager uiManager = UIManager.Instance;
            LevelData startingLevelData = levels[_currentLevelIndex];
            StateMachine.ChangeState(new PlayingState(this, startingLevelData.levelDuration, uiManager, startingLevelData));
        }

        public void OnNextLevelButtonClicked()
        {
            _currentLevelIndex++;
            if (_currentLevelIndex < levels.Length)
            {
                SetLevelParameters(_currentLevelIndex);
                SaveCurrentLevel();
                UIManager uiManager = UIManager.Instance;
                LevelData nextLevelData = levels[_currentLevelIndex];
                StateMachine.ChangeState(new PlayingState(this, nextLevelData.levelDuration, uiManager, nextLevelData));
            }
            else
            {
                Debug.Log("Tüm seviyeler tamamlandı!");
                _currentLevelIndex = 0;
                SaveCurrentLevel();
            }
        }

        private void SetLevelParameters(int levelIndex)
        {
            if (levelIndex < levels.Length)
            {
                levelDuration = levels[levelIndex].levelDuration;
                spawnManager.SetLevelParameters(levels[levelIndex]);
            }
        }


        public void IncrementDefeatedEnemies()
        {
            int totalDefeatedEnemies = PlayerPrefs.GetInt("TotalDefeatedEnemies", 0);
            totalDefeatedEnemies++;
            _sessionDefeatedEnemies++;
            UIManager.Instance.UpdateEnemiesDefeated(totalDefeatedEnemies);
            PlayerPrefs.SetInt("TotalDefeatedEnemies", totalDefeatedEnemies);
            PlayerPrefs.Save();
        }

        private void LoadDefeatedEnemies()
        {
            _defatedTotalEnemiesCount = PlayerPrefs.GetInt("TotalDefeatedEnemies", 0);
            UIManager.Instance.UpdateEnemiesDefeated(_defatedTotalEnemiesCount);
        }
        public void OnPlayerDeath()
        {
            StateMachine.ChangeState(new GameOverState(this));
        }
        
        public void RestartGame()
        {
            UIManager.Instance.HideGameOver();
            UIManager.Instance.HideGameWon();
            _sessionDefeatedEnemies = 0;
            LevelData startingLevelData = levels[_currentLevelIndex];
            StateMachine.ChangeState(new PlayingState(this, startingLevelData.levelDuration, UIManager.Instance, startingLevelData));
        }
    }
}