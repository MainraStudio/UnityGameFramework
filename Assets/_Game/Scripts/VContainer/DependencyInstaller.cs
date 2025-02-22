using VContainer;
using VContainer.Unity;
using UnityEngine;
using System;

public interface IFactory { }

public class DependencyInstaller : LifetimeScope
{
    [SerializeField] private DependencyConfiguration config;

    protected override void Configure(IContainerBuilder builder)
    {
        if (config == null)
        {
            Debug.LogError("[VContainer] Configuration asset not assigned!");
            return;
        }

        foreach (var registration in config.Registrations)
        {
            try
            {
                RegisterDependency(builder, registration);
            }
            catch (Exception e)
            {
                Debug.LogError($"[VContainer] Registration failed for {registration.RegistrationType}: {e.Message}");
            }
        }
    }

    private void RegisterDependency(IContainerBuilder builder, RegistrationData registration)
    {
        var lifetime = ConvertLifetime(registration.registrationLifetime);

        switch (registration.RegistrationType)
        {
            case RegistrationType.Class:
                RegisterClass(builder, registration, lifetime);
                break;

            case RegistrationType.Interface:
                RegisterInterface(builder, registration, lifetime);
                break;

            case RegistrationType.ComponentInScene:
                RegisterComponentInScene(builder, registration, lifetime);
                break;

            case RegistrationType.Instance:
                RegisterInstance(builder, registration, lifetime);
                break;

            case RegistrationType.ComponentInPrefab:
                RegisterComponentInPrefab(builder, registration, lifetime);
                break;

            case RegistrationType.Factory:
                RegisterFactory(builder, registration, lifetime);
                break;

            case RegistrationType.EntryPoint:
                RegisterEntryPoint(builder, registration);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(registration.RegistrationType),
                    $"Unhandled registration type: {registration.RegistrationType}");
        }
    }

    private void RegisterClass(IContainerBuilder builder, RegistrationData reg, Lifetime lifetime)
    {
        var type = reg.ImplementationType?.GetClass();
        if (type == null)
        {
            Debug.LogError("Class registration requires valid implementation type");
            return;
        }
        builder.Register(type, lifetime);
    }

    private void RegisterInterface(IContainerBuilder builder, RegistrationData reg, Lifetime lifetime)
    {
        var interfaceType = reg.InterfaceType?.GetClass();
        var implType = reg.ImplementationType?.GetClass();

        if (interfaceType == null || implType == null)
        {
            Debug.LogError("Interface registration requires both interface and implementation types");
            return;
        }

        builder.Register(implType, lifetime).As(interfaceType);
    }

    private void RegisterComponentInScene(IContainerBuilder builder, RegistrationData reg, Lifetime lifetime)
    {
        var componentType = reg.ImplementationType?.GetClass();
        if (componentType == null || reg.SceneObject == null)
        {
            Debug.LogError("Component registration requires component type and scene object");
            return;
        }

        var component = reg.SceneObject.GetComponent(componentType);
        if (component == null)
        {
            Debug.LogError($"Component {componentType.Name} not found on {reg.SceneObject.name}");
            return;
        }

        builder.RegisterComponent(component).AsImplementedInterfaces();
    }

    private void RegisterInstance(IContainerBuilder builder, RegistrationData reg, Lifetime lifetime)
    {
        if (reg.Instance == null)
        {
            Debug.LogError("Instance registration requires an instance reference");
            return;
        }

        builder.RegisterInstance(reg.Instance).AsImplementedInterfaces();
    }

    private void RegisterComponentInPrefab(IContainerBuilder builder, RegistrationData reg, Lifetime lifetime)
    {
        var componentType = reg.ImplementationType?.GetClass();
        if (componentType == null || reg.Prefab == null)
        {
            Debug.LogError("Prefab registration requires component type and prefab");
            return;
        }

        var component = reg.Prefab.GetComponent(componentType);
        if (component == null)
        {
            Debug.LogError($"Component {componentType.Name} not found on prefab {reg.Prefab.name}");
            return;
        }

        builder.RegisterComponentInNewPrefab(component, lifetime)
            .UnderTransform(transform);
    }

    private void RegisterFactory(IContainerBuilder builder, RegistrationData reg, Lifetime lifetime)
    {
        var factoryType = reg.FactoryType?.GetClass();
        if (factoryType == null)
        {
            Debug.LogError("Factory registration requires factory type");
            return;
        }

        if (!typeof(IFactory).IsAssignableFrom(factoryType))
        {
            Debug.LogError($"{factoryType.Name} must implement IFactory interface");
            return;
        }

        builder.Register(factoryType, lifetime);
    }

    private void RegisterEntryPoint(IContainerBuilder builder, RegistrationData reg)
    {
        var entryPointType = reg.ImplementationType?.GetClass();
        if (entryPointType == null)
        {
            Debug.LogError("Entry point registration requires implementation type");
            return;
        }

        if (!typeof(IStartable).IsAssignableFrom(entryPointType) &&
            !typeof(ITickable).IsAssignableFrom(entryPointType))
        {
            Debug.LogError($"{entryPointType.Name} must implement IStartable/ITickable");
            return;
        }

        builder.RegisterEntryPoint(r => (IStartable)r.Resolve(entryPointType), Lifetime.Scoped);
    }

    private Lifetime ConvertLifetime(RegistrationLifetimeScope scope)
    {
        return scope switch
        {
            RegistrationLifetimeScope.Transient => Lifetime.Transient,
            RegistrationLifetimeScope.Scoped => Lifetime.Scoped,
            RegistrationLifetimeScope.Singleton => Lifetime.Singleton,
            _ => throw new ArgumentOutOfRangeException(nameof(scope), $"Unknown lifetime: {scope}")
        };
    }
}