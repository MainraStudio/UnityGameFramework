using _Game.Scripts.Application.Manager.Core.UISystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
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

		private UIManager uIManager;

		protected override void Awake()
		{
			base.Awake();
			uIManager = UIManager.Instance;

			play.onClick.AddListener(OnPlayClicked);
			settings.onClick.AddListener(OnSettingsClicked);
			quit.onClick.AddListener(OnQuitClicked);
			backButton.onClick.AddListener(OnBackClicked);
		}

		private void OnPlayClicked()
		{
			// Implement play button functionality
			Debug.Log("Play button clicked");
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