using _Game.Scripts.Application.Manager.Core.GameSystem.Interfaces;
using UnityEngine;
namespace _Game.Scripts.Application.Manager.Core.States
{
    public class ResultState : GameState
    {
        public override void EnterState()
        {
            Debug.Log("Entering Result State");
            // Logika untuk memasuki state
        }

        public override void UpdateState()
        {
            // Logika Update Result
        }

        public override void ExitState()
        {
            Debug.Log("Exiting Result State");
            // Logika keluar dari Result
        }
    }
}