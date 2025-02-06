using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
	private static Dictionary<Type, object> services = new Dictionary<Type, object>();

	public static void Register<T>(T service) where T : class
	{
		Type type = typeof(T);
		if (!services.ContainsKey(type))
		{
			services[type] = service;
			Debug.Log($"[ServiceLocator] {type.Name} terdaftar.");
		}
		else
		{
			Debug.LogWarning($"[ServiceLocator] {type.Name} sudah terdaftar sebelumnya.");
		}
	}

	public static T Get<T>() where T : class
	{
		Type type = typeof(T);
		if (services.TryGetValue(type, out var service))
		{
			return service as T;
		}
		else
		{
			Debug.LogError($"[ServiceLocator] {type.Name} belum terdaftar!");
			return null;
		}
	}

	public static void Unregister<T>() where T : class
	{
		Type type = typeof(T);
		if (services.ContainsKey(type))
		{
			services.Remove(type);
			Debug.Log($"[ServiceLocator] {type.Name} dihapus.");
		}
	}

	public static void Clear()
	{
		services.Clear();
		Debug.Log("[ServiceLocator] Semua service dihapus.");
	}
}