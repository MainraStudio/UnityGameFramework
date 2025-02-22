using UnityEditor;
using UnityEngine;

public enum RegistrationLifetime
{
	Transient,
	Scoped,
	Singleton
}

[System.Serializable]
public class RegistrationData
{
	public MonoScript InterfaceType; // Ganti nama dari RegisteredType
	public MonoScript ImplementationType;
	public RegistrationLifetime Lifetime; // Gunakan enum yang sudah di-rename
}