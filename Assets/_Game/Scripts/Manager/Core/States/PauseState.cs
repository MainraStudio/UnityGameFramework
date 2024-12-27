using UnityEngine;

public class PauseState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entering Pause State");
        Time.timeScale = 0; // Pause game dengan menghentikan waktu
    }

    public void UpdateState()
    {
        // Logika pause, misalnya mendeteksi input untuk melanjutkan permainan
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Resuming Game...");
            Time.timeScale = 1; // Lanjutkan permainan
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Pause State");
        Time.timeScale = 1; // Pastikan waktu kembali normal
    }
}