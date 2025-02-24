using MainraFramework;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameRegistrationLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {

        //README: Register services here

    }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
