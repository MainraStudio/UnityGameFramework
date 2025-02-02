using UnityEngine;

namespace MainraFramework.States
{
    public class GameplayState : GameState
    {
        public override void EnterState()
        {
            Debug.Log("Entering Gameplay State");
            // Mulai gameplay, seperti memulai timer atau musuh
        }

        public override void UpdateState()
        {
            // Logika gameplay, misalnya mendeteksi input atau mengelola musuh
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Pausing Game...");
                // Panggil state pause jika ada
            }
        }

        public override void ExitState()
        {
            Debug.Log("Exiting Gameplay State");
            // Logika keluar dari gameplay, seperti menghentikan musuh atau menyimpan progres
        }
    }
}
