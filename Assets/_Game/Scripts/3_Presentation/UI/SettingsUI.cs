using System;
using _Game.Scripts.GameConfiguration;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _volumeMaster;
    [SerializeField] private Slider _volumeMusic;
    [SerializeField] private Slider _volumeSFX;
    
    [Inject] private GameConfig _gameConfig;
    private GameSettings _gameSettings; // Referensi cache
    
    private void Awake()
    {
        _gameSettings = _gameConfig.GameSettings; // Cache referensi
        _gameSettings.LoadSettings();
    }
    
    private void Start()
    {
        InitializeSliderValues();
        SetupEventListeners();
    }
    
    private void InitializeSliderValues()
    {
        _volumeMaster.value = _gameSettings.masterVolume;
        _volumeMusic.value = _gameSettings.musicVolume;
        _volumeSFX.value = _gameSettings.sfxVolume;
    }
    
    private void SetupEventListeners()
    {
        _volumeMaster.onValueChanged.AddListener(UpdateMasterVolume);
        _volumeMusic.onValueChanged.AddListener(UpdateMusicVolume);
        _volumeSFX.onValueChanged.AddListener(UpdateSFXVolume);
    }
    
    private void UpdateMasterVolume(float value)
    {
        _gameSettings.masterVolume = value;
    }
    
    private void UpdateMusicVolume(float value)
    {
        _gameSettings.musicVolume = value;
    }
    
    private void UpdateSFXVolume(float value)
    {
        _gameSettings.sfxVolume = value;
    }
    
    private void OnDisable()
    {
        SaveAndApplySettings();
        RemoveEventListeners();
    }
    
    private void SaveAndApplySettings()
    {
        _gameSettings.SaveSettings();
        _gameSettings.ApplySettings();
    }
    
    private void RemoveEventListeners()
    {
        _volumeMaster.onValueChanged.RemoveListener(UpdateMasterVolume);
        _volumeMusic.onValueChanged.RemoveListener(UpdateMusicVolume);
        _volumeSFX.onValueChanged.RemoveListener(UpdateSFXVolume);
    }
}