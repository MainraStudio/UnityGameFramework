using _Game.Scripts.Application.Manager.Core.GameSystem.Interfaces;
using _Game.Scripts.Application.Manager.Core.States;
namespace _Game.Scripts.Application.Manager.Core.GameSystem
{
	public class GameManager : PersistentSingleton<GameManager>
	{
		private GameState currentState;
		
		public SceneManager SceneManager { get; private set; }
		public SaveDataManager SaveDataManager { get; private set; }
		//TODO: Add more manager here

		protected override void Awake()
		{
			base.Awake();
			SceneManager = new SceneManager(this);
			SaveDataManager = new SaveDataManager(this);
			//TODO: Initialize more manager here
		}
		public void Initialize()
		{
			SceneManager = new SceneManager(this);
			SaveDataManager = new SaveDataManager(this);
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
		
		public void QuitGame()
		{
			UnityEngine.Application.Quit();
			ServiceLocator.Clear();
		}
	}
}