using _Game.Scripts.Core.Interfaces;
using _Game.Scripts.Core.Enums;
using _Game.Scripts.GameConfiguration;
using CarterGames.Assets.SaveManager;
using DebugToolsPlus;
using UnityEngine;
using VContainer.Unity;
using MainraFramework.Parameter;
using VContainer;

public class GameBootstrapper : MonoBehaviour, IInitializable
{
    // Remove these serialized fields as we'll use GameSettings instead
    // [SerializeField] private float targetFrameRate = 60f;
    // [SerializeField] private bool vsync = true;
    
    private readonly ISceneLoader _sceneLoader;
    private readonly IGameStateService _gameStateService;
    
    [Inject] private GameConfig gameConfig;

    public GameBootstrapper(ISceneLoader sceneLoader, IGameStateService gameStateService)
    {
        _sceneLoader = sceneLoader;
        _gameStateService = gameStateService;
    }

    public void Initialize()
    {
        Application.targetFrameRate = gameConfig.GameSettings.targetFrameRate;
        QualitySettings.vSyncCount = gameConfig.GameSettings.vSync ? 1 : 0;
        _gameStateService.SetState(GameState.Menu);
        _sceneLoader.LoadSceneThroughLoadingAsync(Parameter.Scenes.MAINMENU);
    }
}