// DependencyInstaller.cs
using VContainer;
using VContainer.Unity;
using UnityEngine;

public class DependencyInstaller : LifetimeScope
{
	[SerializeField] private DependencyConfiguration dependencyConfig; // Ganti nama variabel

	protected override void Configure(IContainerBuilder builder)
	{
		if (dependencyConfig == null) return;

		foreach (var registration in dependencyConfig.Registrations)
		{
			var interfaceType = registration.InterfaceType?.GetClass();
			var implementationType = registration.ImplementationType?.GetClass();

			if (interfaceType == null || implementationType == null) continue;

			// Mapping ke Lifetime milik VContainer
			var lifetime = registration.Lifetime switch
			{
				RegistrationLifetime.Transient => Lifetime.Transient,
				RegistrationLifetime.Scoped => Lifetime.Scoped,
				RegistrationLifetime.Singleton => Lifetime.Singleton,
				_ => Lifetime.Singleton
			};

			builder.Register(implementationType, lifetime).As(interfaceType);
		}
	}
}