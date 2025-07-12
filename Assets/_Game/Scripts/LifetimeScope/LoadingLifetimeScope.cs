using _Game.Scripts.Presentation.UI;
using VContainer;
using VContainer.Unity;

public class LoadingLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<LoadingSceneManager>();
    }
}
