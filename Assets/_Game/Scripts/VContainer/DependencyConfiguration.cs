using UnityEngine;

[CreateAssetMenu(fileName = "DependencyConfig", menuName = "VContainer/Dependency Configuration")]
public class DependencyConfiguration : ScriptableObject
{
	public RegistrationData[] Registrations;
}