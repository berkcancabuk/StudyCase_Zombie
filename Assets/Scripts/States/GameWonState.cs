using Managers;
using States.Interface;
using Unity.VisualScripting;
using UnityEngine;

public class GameWonState : IGameState
{
    private GameManager _gameManager;
    private UIManager _uiManager;

    public GameWonState(GameManager gameManager,UIManager uiManager)
    {
        _gameManager = gameManager;
        _uiManager = uiManager;
    }

    public void Enter()
    {
        // UI'ı göster
        _uiManager.ShowGameWon(_gameManager._sessionDefeatedEnemies,_gameManager._defatedTotalEnemiesCount);
        _gameManager._sessionDefeatedEnemies = 0;
        // Tüm aktif düşmanları temizle
        ClearActiveEnemies();
        
        // Player controller'ı devre dışı bırak
        DisablePlayerController();
        
        // Zamanı durdur
        Time.timeScale = 0;
    }

    private void ClearActiveEnemies()
    {
        // Sahnedeki tüm aktif düşmanları bul
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }

    private void DisablePlayerController()
    {
        if (_gameManager.playerController != null)
        {
            _gameManager.playerController.enabled = false;
        }
    }

    public void Exit()
    {
        // Zamanı normale döndür
        Time.timeScale = 1;
    }

    public void Update()
    {
        // GameWonState update logic
    }
}