using System;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Application.Manager.Core.GameSystem
{
    public static class ServiceLocator
    {
        private static readonly object _lock = new object();
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Register<T>(T service, bool allowOverride = false) where T : class
        {
            lock (_lock)
            {
                Type type = typeof(T);
                if (!services.ContainsKey(type))
                {
                    services[type] = service;
                    Log($"[ServiceLocator] {type.Name} terdaftar.");
                }
                else if (allowOverride)
                {
                    services[type] = service;
                    Log($"[ServiceLocator] {type.Name} di-override.");
                }
                else
                {
                    LogWarning($"[ServiceLocator] {type.Name} sudah terdaftar.");
                }
            }
        }

        public static T Get<T>() where T : class
        {
            lock (_lock)
            {
                Type type = typeof(T);
                if (services.TryGetValue(type, out var service))
                {
                    return service as T;
                }
                throw new InvalidOperationException($"[ServiceLocator] {type.Name} belum terdaftar!");
            }
        }

        public static void Unregister<T>() where T : class
        {
            lock (_lock)
            {
                Type type = typeof(T);
                if (services.Remove(type))
                {
                    Log($"[ServiceLocator] {type.Name} dihapus.");
                }
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                services.Clear();
                Log("[ServiceLocator] Semua service dihapus.");
            }
        }

        // Helper untuk logging yang aman di production
        private static void Log(string message)
        {
        #if UNITY_EDITOR
            Debug.Log(message);
        #endif
        }

        private static void LogWarning(string message)
        {
        #if UNITY_EDITOR
            Debug.LogWarning(message);
        #endif
        }
    }
}