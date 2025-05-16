using UnityEngine;
using UnityEngine.PlayerLoop;
using VContainer.Unity;

namespace _Game.Scripts.GameConfiguration
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Game Settings")]
    public class GameSettings : ScriptableObject, IInitializable
    {
        [Header("Sound Settings")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        [Range(0f, 1f)]
        public float musicVolume = 1f;
        [Range(0f, 1f)]
        public float sfxVolume = 1f;
        public bool muteAudio = false;

        [Header("Graphics Settings")]
        [Tooltip("Quality level index from QualitySettings")]
        public int qualityLevel = 2;
        public bool fullScreen = true;
        public int targetFrameRate = 60;
        public bool vSync = true;
        
        [Header("Resolution")]
        public int resolutionIndex = 0;

        public void Initialize()
        {
            LoadSettings();
        }

        // Method to apply settings
        public void ApplySettings()
        {
            // Apply Audio Settings
            AudioListener.volume = masterVolume;
            
            // Apply Graphics Settings
            QualitySettings.SetQualityLevel(qualityLevel);
            Screen.fullScreen = fullScreen;
            UnityEngine.Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = vSync ? 1 : 0;
            
            // Apply resolution (would need resolution list to implement fully)
            if (Screen.resolutions.Length > 0 && resolutionIndex < Screen.resolutions.Length)
            {
                Resolution resolution = Screen.resolutions[resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, fullScreen);
            }
        }

        // Save settings to PlayerPrefs
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetInt("MuteAudio", muteAudio ? 1 : 0);
            
            PlayerPrefs.SetInt("QualityLevel", qualityLevel);
            PlayerPrefs.SetInt("FullScreen", fullScreen ? 1 : 0);
            PlayerPrefs.SetInt("TargetFrameRate", targetFrameRate);
            PlayerPrefs.SetInt("VSync", vSync ? 1 : 0);
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
            
            PlayerPrefs.Save();
        }

        // Load settings from PlayerPrefs
        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey("MasterVolume"))
            {
                masterVolume = PlayerPrefs.GetFloat("MasterVolume");
                musicVolume = PlayerPrefs.GetFloat("MusicVolume");
                sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
                muteAudio = PlayerPrefs.GetInt("MuteAudio") == 1;
                
                qualityLevel = PlayerPrefs.GetInt("QualityLevel");
                fullScreen = PlayerPrefs.GetInt("FullScreen") == 1;
                targetFrameRate = PlayerPrefs.GetInt("TargetFrameRate");
                vSync = PlayerPrefs.GetInt("VSync") == 1;
                resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
                
                ApplySettings();
            }
        }
    }
}