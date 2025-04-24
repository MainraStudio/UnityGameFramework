using _Game.Scripts.Core.Interfaces;
using _Game.Scripts.Core.Enums;
using UnityEngine;
using VContainer.Unity;
using MainraFramework.Parameter;
public class GameBootstrapper : IInitializable
{
    private readonly ISceneLoader _sceneLoader;
    private readonly IGameStateService _gameStateService;

    public GameBootstrapper(ISceneLoader sceneLoader, IGameStateService gameStateService)
    {
        _sceneLoader = sceneLoader;
        _gameStateService = gameStateService;
    }

    public void Initialize()
    {
        _gameStateService.SetState(GameState.Menu);
        _sceneLoader.LoadSceneThroughLoadingAsync(Parameter.Scenes.MAINMENU);
    }
}
