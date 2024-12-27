using MainraFramework;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : BaseUI
{
    [SerializeField] private Button play;
    [SerializeField] private Button settings;
    [SerializeField] private Button quit;

    private UIManager uIManager;

    protected override void Awake()
    {
        base.Awake();
        uIManager = UIManager.Instance;

        play.onClick.AddListener(OnPlayClicked);
        settings.onClick.AddListener(OnSettingsClicked);
        quit.onClick.AddListener(OnQuitClicked);
    }

    private void OnPlayClicked()
    {
        // Implement play button functionality
        Debug.Log("Play button clicked");
    }

    private void OnSettingsClicked()
    {
        // Implement settings button functionality
        Debug.Log("Settings button clicked");
    }

    private void OnQuitClicked()
    {
        // Implement quit button functionality
        Debug.Log("Quit button clicked");
        Application.Quit();
    }
}