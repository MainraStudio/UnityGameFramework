using System.Collections.Generic;
using UnityEngine;
namespace _Game.Scripts.Application.Manager.Core
{
    public abstract class BaseFactory<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Header("Factory Settings")]
        [SerializeField] private T prefab; // Prefab utama
        [SerializeField] private bool usePooling = true; // Menggunakan pooling atau tidak
        [SerializeField] private int initialPoolSize = 10; // Ukuran awal pool (opsional jika pooling diaktifkan)
        [SerializeField] private int poolGrowthSize = 5; // Jumlah objek baru yang ditambahkan saat pool habis

        private Queue<T> pool = new Queue<T>();

        // Callback lifecycle
        public event System.Action<T> OnObjectCreated; // Dipanggil ketika objek baru dibuat
        public event System.Action<T> OnObjectActivated; // Dipanggil ketika objek diaktifkan
        public event System.Action<T> OnObjectDeactivated; // Dipanggil ketika objek dinonaktifkan

        private void Awake()
        {
            if (prefab == null)
            {
                Debug.LogError($"{GetType().Name}: Prefab is not set.");
                return;
            }

            if (usePooling)
            {
                InitializePool();
            }
        }

        /// <summary>
        /// Menginisialisasi pool jika pooling diaktifkan.
        /// </summary>
        private void InitializePool()
        {
            AddToPool(initialPoolSize); // Tambahkan objek awal ke pool
        }

        /// <summary>
        /// Menambahkan sejumlah objek baru ke pool.
        /// </summary>
        private void AddToPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T instance = InstantiateNewInstance();
                Recycle(instance); // Masukkan objek ke pool
            }
        }

        /// <summary>
        /// Membuat atau mengambil instance dari pool.
        /// </summary>
        /// <param name="position">Posisi spawn.</param>
        /// <param name="rotation">Rotasi spawn.</param>
        /// <returns>Instance objek yang aktif.</returns>
        public virtual T Create(Vector3 position, Quaternion rotation)
        {
            T instance;

            if (usePooling)
            {
                // Jika pool kosong, tambahkan objek baru ke pool
                if (pool.Count == 0)
                {
                    Debug.LogWarning($"{GetType().Name}: Pool is empty, adding more objects.");
                    AddToPool(poolGrowthSize); // Tambahkan sejumlah objek baru
                }

                instance = pool.Dequeue();
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = InstantiateNewInstance();
                instance.transform.position = position;
                instance.transform.rotation = rotation;
            }

            OnObjectActivated?.Invoke(instance); // Callback saat objek diaktifkan
            return instance;
        }

        /// <summary>
        /// Membuat objek baru tanpa menggunakan pool.
        /// </summary>
        private T InstantiateNewInstance()
        {
            if (prefab == null)
            {
                Debug.LogError($"{GetType().Name}: Prefab is not set.");
                return null;
            }

            T instance = Instantiate(prefab);
            OnObjectCreated?.Invoke(instance); // Callback saat objek dibuat
            return instance;
        }

        /// <summary>
        /// Mengembalikan objek ke pool untuk digunakan kembali.
        /// </summary>
        /// <param name="instance">Objek yang akan direcycle.</param>
        public virtual void Recycle(T instance)
        {
            if (!usePooling)
            {
                Destroy(instance.gameObject);
                return;
            }

            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
            OnObjectDeactivated?.Invoke(instance); // Callback saat objek dinonaktifkan
        }

        /// <summary>
        /// Mengganti prefab yang digunakan factory.
        /// </summary>
        /// <param name="newPrefab">Prefab baru.</param>
        public void SetPrefab(T newPrefab)
        {
            prefab = newPrefab;
        }

        /// <summary>
        /// Menghapus semua objek di pool.
        /// </summary>
        public void ClearPool()
        {
            while (pool.Count > 0)
            {
                T instance = pool.Dequeue();
                Destroy(instance.gameObject);
            }
        }

        /// <summary>
        /// Mendapatkan jumlah objek dalam pool.
        /// </summary>
        public int GetPoolSize()
        {
            return pool.Count;
        }
    }
}