using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum RegistrationType
{
	Class,
	Interface,
	ComponentInScene,
	Instance,
	ComponentInPrefab,
	Factory,
	EntryPoint
}

public enum RegistrationLifetimeScope
{
	Transient,
	Scoped,
	Singleton
}

[System.Serializable]
public class RegistrationData
{
	public RegistrationType RegistrationType;
	public MonoScript InterfaceType;
	public MonoScript ImplementationType;
	public GameObject SceneObject;
	public UnityEngine.Object Instance;
	public GameObject Prefab;
	public MonoScript FactoryType;
	[FormerlySerializedAs("Lifetime")] public RegistrationLifetimeScope registrationLifetime;
}