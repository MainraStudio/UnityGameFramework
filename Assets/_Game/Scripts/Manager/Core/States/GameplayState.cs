using UnityEngine;

namespace MainraFramework.States
{
    public class GameplayState : IGameState
    {
        public void EnterState()
        {
            Debug.Log("Entering Gameplay State");
            // Mulai gameplay, seperti memulai timer atau musuh
        }

        public void UpdateState()
        {
            // Logika gameplay, misalnya mendeteksi input atau mengelola musuh
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Pausing Game...");
                // Panggil state pause jika ada
            }
        }

        public void ExitState()
        {
            Debug.Log("Exiting Gameplay State");
            // Logika keluar dari gameplay, seperti menghentikan musuh atau menyimpan progres
        }
    }
}
