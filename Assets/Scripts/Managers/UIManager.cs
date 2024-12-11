using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("Main Menu")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private Button startButton;

        [Header("Game Won")]
        [SerializeField] private GameObject gameWonPanel;
        [SerializeField] private TMP_Text scoreText,totalScoreText;
        [SerializeField] private Button nextLevelButton;

        [Header("Gameplay UI")]
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text enemiesDefeatedText;
        [SerializeField] private TMP_Text waveCountText;
        
        [Header("GameOver UI")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverTotalScoreText;
        [SerializeField] private TextMeshProUGUI gameOverSessionScoreText;
        [SerializeField] private Button restartButton;
        
        [SerializeField] private GameObject joystick;

        public static UIManager Instance { get; private set; }

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
        }

        public void ShowMainMenu()
        {
            mainMenuPanel.SetActive(true);
            joystick.SetActive(false);
        }

        public void HideMainMenu()
        {
            mainMenuPanel.SetActive(false);
            gameplayPanel.SetActive(true);
            joystick.SetActive(true);
        }

        public void ShowGameWon(int score, int totalScore)
        {
            gameplayPanel.SetActive(false);
            gameWonPanel.SetActive(true);
            joystick.SetActive(false);
            scoreText.text = score.ToString();
            totalScoreText.text = score.ToString();
        }

        public void HideGameWon()
        {
            gameWonPanel.SetActive(false);
            gameplayPanel.SetActive(true);
            joystick.SetActive(true);
        }

        public void UpdateTimer(float timeRemaining)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
        


        public void UpdateWaveCount(int remainingEnemies)
        {
            if (waveCountText != null)
            {
                waveCountText.text = $"Remaining Enemies: {remainingEnemies}";
            }
        }

        public void UpdateEnemiesDefeated(int count)
        {
            enemiesDefeatedText.text = "Defeated: " + count;
        }

        public void Initialize(GameManager gameManager)
        {
            startButton.onClick.AddListener(gameManager.OnStartButtonClicked);
            nextLevelButton.onClick.AddListener(gameManager.OnNextLevelButtonClicked);
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
        
        public void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                joystick.SetActive(false);
            
                if (gameOverTotalScoreText != null)
                    gameOverTotalScoreText.text = $"Total Score: {GameManager.Instance._defatedTotalEnemiesCount}";
            
                if (gameOverSessionScoreText != null)
                    gameOverSessionScoreText.text = $"Session Score: {GameManager.Instance._sessionDefeatedEnemies}";
            }
        }
        
        public void HideGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
                joystick.SetActive(true);
            }
        }

        private void OnRestartButtonClicked()
        {
            GameManager.Instance.RestartGame();
        }
    }
}
