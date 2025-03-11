using _Game.Scripts.Application.Manager.Core.GameSystem;
using _Game.Scripts.Application.Manager.Core.GameSystem.Interfaces;
using _Game.Scripts.Application.Manager.Core.UISystem;
using _Game.Scripts.Presentation.UI;
using UnityEngine;
namespace _Game.Scripts.Application.Manager.Core.States
{
    public class MainMenuState : GameState
    {
        public override void EnterState()
        {
            GameManager.Instance.SceneManager.LoadScene(Parameter.Scenes.GAMEPLAY,true,10f);
            UIManager.Instance.ShowPopupUI<MainMenuUI>();
            // Logika untuk memasuki menu utama, misalnya mengaktifkan UI
        }

        public override void UpdateState()
        {
            // Logika untuk interaksi di main menu
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Starting Game...");
                // Anda bisa mengganti state ke gameplay di sini
            }
        }

        public override void ExitState()
        {
            Debug.Log("Exiting Main Menu State");
            // Logika keluar dari main menu, seperti menonaktifkan UI
        }
    }
}