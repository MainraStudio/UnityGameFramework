using System;
using _Game.Scripts.Core.Interfaces;
using _Game.Scripts.Core.Enums;
using CarterGames.Assets.SaveManager;
using UnityEngine;
using VContainer.Unity;
using MainraFramework.Parameter;
public class GameBootstrapper : MonoBehaviour, IInitializable
{
    [SerializeField] private float targetFrameRate = 60f;
    
    private readonly ISceneLoader _sceneLoader;
    private readonly IGameStateService _gameStateService;


    private void Awake()
    {
        Application.targetFrameRate = (int)targetFrameRate;
    }

    public GameBootstrapper(ISceneLoader sceneLoader, IGameStateService gameStateService)
    {
        _sceneLoader = sceneLoader;
        _gameStateService = gameStateService;
    }

    public void Initialize()
    {
        _gameStateService.SetState(GameState.Menu);
        _sceneLoader.LoadSceneThroughLoadingAsync(Parameter.Scenes.MAINMENU);
        SaveManager.Save(true);
    }
}
