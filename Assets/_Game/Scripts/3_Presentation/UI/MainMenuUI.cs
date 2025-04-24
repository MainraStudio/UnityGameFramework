using _Game.Scripts.Core.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using _Game.Scripts.Core.Interfaces;
using MainraFramework.Parameter;
using VContainer;

namespace _Game.Scripts.Presentation.UI

{
	public class MainMenuUI : BaseUI
	{
		[BoxGroup("Buttons")]
		[SerializeField] private Button play;

		[BoxGroup("Buttons")]
		[SerializeField] private Button settings;

		[BoxGroup("Buttons")]
		[SerializeField] private Button quit;

		[BoxGroup("Settings Panel")]
		[SerializeField] private GameObject settingsPanel;

		[BoxGroup("Settings Panel")]
		[SerializeField] private Button backButton;
		
		[Inject]
		private readonly IGameStateService _gameStateService;
		[Inject]
		private readonly ISceneLoader _sceneLoader;

		protected override void Awake()
		{
			base.Awake();

			play.onClick.AddListener(OnPlayClicked);
			settings.onClick.AddListener(OnSettingsClicked);
			quit.onClick.AddListener(OnQuitClicked);
			backButton.onClick.AddListener(OnBackClicked);
		}

		private void OnPlayClicked()
		{
			Debug.Log("Play button clicked");
			_gameStateService.SetState(GameState.Playing);
			_sceneLoader.LoadScene(Parameter.Scenes.GAMEPLAY);
		}

		private void OnSettingsClicked()
		{
			// Show settings panel
			settingsPanel.SetActive(true);
			Debug.Log("Settings button clicked");
		}

		private void OnQuitClicked()
		{
			// Implement quit button functionality
			Debug.Log("Quit button clicked");
			UnityEngine.Application.Quit();
		}

		private void OnBackClicked()
		{
			// Hide settings panel
			settingsPanel.SetActive(false);
			Debug.Log("Back button clicked");
		}
	}
}