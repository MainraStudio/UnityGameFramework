using System;
using MainraFramework.States;
using UnityEngine;

namespace MainraFramework
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        private IGameState currentState;
        public SceneManager SceneManager { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SceneManager = new SceneManager(this);
        }

        public void SetState(IGameState newState)
        {
            if (currentState != null)
                currentState.ExitState(); // Keluar dari state saat ini

            currentState = newState;

            if (currentState != null)
                currentState.EnterState(); // Masuk ke state baru
        }

        private void Start()
        {
            SetState(new MainMenuState()); // Set state awal
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.UpdateState(); // Update state saat ini
            }
        }
    }
}