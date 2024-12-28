using UnityEngine;

namespace MainraFramework.States
{
    public class MainMenuState : IGameState
    {
        public void EnterState()
        {
            UIManager.Instance.ShowPopupUI<MainMenuUI>();
            Debug.Log("Entering Main Menu State");
            // Logika untuk memasuki menu utama, misalnya mengaktifkan UI
        }

        public void UpdateState()
        {
            // Logika untuk interaksi di main menu
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Starting Game...");
                // Anda bisa mengganti state ke gameplay di sini
            }
        }

        public void ExitState()
        {
            Debug.Log("Exiting Main Menu State");
            // Logika keluar dari main menu, seperti menonaktifkan UI
        }
    }
}