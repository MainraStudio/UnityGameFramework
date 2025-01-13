using System;
using MainraFramework.States;
using UnityEngine;

namespace MainraFramework
{
	public class GameManager : PersistentSingleton<GameManager>
	{
		private GameState currentState;
		public SceneManager SceneManager { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			SceneManager = new SceneManager(this);
		}

		public void SetState(GameState newState)
		{
			if (currentState != null)
				currentState.ExitState(); // Exit the current state

			currentState = newState;

			if (currentState != null)
				currentState.EnterState(); // Enter the new state
		}

		private void Start()
		{
			SetState(new MainMenuState()); // Set initial state
		}

		private void Update()
		{
			if (currentState != null)
			{
				currentState.UpdateState(); // Update the current state
			}
		}
	}
}