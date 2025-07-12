using _Game.Scripts.Application;
using _Game.Scripts.Core.Interfaces;
using _Game.Scripts.GameConfiguration;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameConfig _gameConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameBootstrapper>();


        // Core Layer
        builder.Register<EventAggregator>(Lifetime.Singleton).As<IEventAggregator>();
        builder.Register<IGameStateService, GameStateService>(Lifetime.Singleton);
        
        // Scene Management
        builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);
        

        // Infrastructure Layer
        builder.RegisterInstance(_gameConfig).As<GameConfig>();

        // Cross-layer dependencies
        //builder.Register<IAssetLoader, AddressablesLoader>(Lifetime.Singleton);
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
}
