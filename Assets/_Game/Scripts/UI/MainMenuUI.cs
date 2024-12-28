using Ami.BroAudio;
using Ami.BroAudio.Data;
using MainraFramework;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MainMenuUI : BaseUI
{
    [FoldoutGroup("Main Buttons"), SerializeField] private Button play;
    [FoldoutGroup("Main Buttons"), SerializeField] private Button settings;
    [FoldoutGroup("Main Buttons"), SerializeField] private Button quit;

    [FoldoutGroup("Settings Panel"), SerializeField] private GameObject settingsPanel;
    [FoldoutGroup("Settings Panel"), SerializeField] private Slider masterVolumeSlider;
    [FoldoutGroup("Settings Panel"), SerializeField] private Slider musicVolumeSlider;
    [FoldoutGroup("Settings Panel"), SerializeField] private Slider sfxVolumeSlider;
    [FoldoutGroup("Settings Panel"), SerializeField] private Button backButton;

    private UIManager uIManager;

    protected override void Awake()
    {
        base.Awake();
        uIManager = UIManager.Instance;

        play.onClick.AddListener(OnPlayClicked);
        settings.onClick.AddListener(OnSettingsClicked);
        quit.onClick.AddListener(OnQuitClicked);
        backButton.onClick.AddListener(OnBackClicked);
        
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    private void OnPlayClicked()
    {
        // Implement play button functionality
        Debug.Log("Play button clicked");
    }
    
    private void OnMasterVolumeChanged(float value)
    {
        // Implement master volume slider functionality
        Debug.Log("Master volume changed to: " + value);
        BroAudio.SetVolume(value);
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        BroAudio.SetVolume(BroAudioType.Music, value);
    }

    
    private void OnSFXVolumeChanged(float value)
    {
        // Implement SFX volume slider functionality
        Debug.Log("SFX volume changed to: " + value);
        BroAudio.SetVolume(BroAudioType.SFX, value);
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
        Application.Quit();
    }

    private void OnBackClicked()
    {
        // Hide settings panel
        settingsPanel.SetActive(false);
        Debug.Log("Back button clicked");
    }
}