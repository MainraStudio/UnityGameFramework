using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this; // Inisialisasi Singleton
            DontDestroyOnLoad(gameObject); // Membuat GameObject tetap hidup di antara scene
        }
        else
        {
            Debug.LogWarning($"Duplicate instance of {typeof(T).Name} detected. Destroying the new one.");
            Destroy(gameObject); // Hancurkan instance baru
        }
    }
}