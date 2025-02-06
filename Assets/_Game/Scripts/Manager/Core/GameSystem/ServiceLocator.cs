using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
	private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
	private static readonly object lockObject = new object();

	public static void Register<T>(T service) where T : class
	{
		Type type = typeof(T);
		lock (lockObject)
		{
			if (!services.ContainsKey(type))
			{
				services[type] = service;
			}
			else
			{
				Debug.LogWarning($"Service {type} is already registered!");
			}
		}
	}

	public static T Get<T>() where T : class
	{
		Type type = typeof(T);
		lock (lockObject)
		{
			if (services.TryGetValue(type, out var service))
			{
				return service as T;
			}
			else
			{
				throw new InvalidOperationException($"Service {type} is not registered!");
			}
		}
	}

	public static void Unregister<T>() where T : class
	{
		Type type = typeof(T);
		lock (lockObject)
		{
			if (services.ContainsKey(type))
			{
				services.Remove(type);
			}
		}
	}
}