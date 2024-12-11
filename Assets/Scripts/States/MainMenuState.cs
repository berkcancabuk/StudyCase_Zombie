using Managers;
using States.Interface;
using UnityEngine;

namespace States
{
    public class MainMenuState : IGameState
    {
        private readonly GameManager _gameManager;

        public MainMenuState(GameManager gm)
        {
            _gameManager = gm;
        }

        public void Enter()
        {
            Debug.Log("MainMenu State: Enter");
            // MainMenu UI’ını aç
            _gameManager.ShowMainMenuUI();
        }

        public void Update()
        {
            // Kullanıcıdan buton beklenir, bu yüzden burada bir şey yapmaya gerek yok
        }

        public void Exit()
        {
            Debug.Log("MainMenu State: Exit");
            // MainMenu UI’ını kapat
            _gameManager.HideMainMenuUI();
        }
    }
}