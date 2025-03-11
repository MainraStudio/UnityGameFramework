using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using _Game.Scripts.Application.Manager.Core.GameSystem;
using UnityEngine;
namespace _Game.Scripts.Application.Manager.Core
{
    public class SaveDataManager
    {
        private static Dictionary<string, object> cache = new Dictionary<string, object>();
        private const string VersionKey = "SaveDataVersion";

        public static event Action<string> OnDataSaved;
        public static event Action<string> OnDataLoaded;
        public static event Action<string> OnDataDeleted;

        private readonly GameManager _gameManager;
        public SaveDataManager(GameManager gameManager)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
        }

        // === Basic Functions with Fluent API ===
        public static void Set(string key, object value)
        {
            try
            {
                string json = JsonUtility.ToJson(new SerializableObject(value));
                PlayerPrefs.SetString(key, json);
                cache[key] = value;
                PlayerPrefs.Save();
                OnDataSaved?.Invoke(key);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error setting PlayerPrefs for key {key}: {ex.Message}");
            }
        }

        public static T Get<T>(string key, T defaultValue = default)
        {
            try
            {
                if (cache.TryGetValue(key, out object cachedValue))
                {
                    return (T)cachedValue;
                }

                if (PlayerPrefs.HasKey(key))
                {
                    string json = PlayerPrefs.GetString(key);
                    SerializableObject serializableObject = JsonUtility.FromJson<SerializableObject>(json);
                    T value = (T)serializableObject.Value;
                    cache[key] = value;
                    OnDataLoaded?.Invoke(key);
                    return value;
                }

                return defaultValue;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting PlayerPrefs for key {key}: {ex.Message}");
                return defaultValue;
            }
        }

        public static void Delete(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                cache.Remove(key);
                OnDataDeleted?.Invoke(key);
            }
        }

        public static void ClearAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            cache.Clear();
        }

        // === JSON Functions ===
        public static void SetObject<T>(string key, T obj)
        {
            string json = JsonUtility.ToJson(obj);
            Set(key, json);
        }

        public static T GetObject<T>(string key, T defaultValue = default)
        {
            string json = Get(key, "");
            return !string.IsNullOrEmpty(json) ? JsonUtility.FromJson<T>(json) : defaultValue;
        }

        // === Batch Save/Load ===
        public static void SetBatch(Dictionary<string, object> data)
        {
            try
            {
                foreach (var pair in data)
                {
                    Set(pair.Key, pair.Value);
                }
                PlayerPrefs.Save(); // Call save after batch operation
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving batch: {ex.Message}");
            }
        }

        public static Dictionary<string, object> GetBatch(List<string> keys)
        {
            var result = new Dictionary<string, object>();
            try
            {
                foreach (var key in keys)
                {
                    if (PlayerPrefs.HasKey(key))
                    {
                        result[key] = Get<object>(key);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting batch: {ex.Message}");
            }
            return result;
        }

        // === Expiration Feature ===
        public static void SetWithExpiration(string key, string value, TimeSpan expiration)
        {
            try
            {
                Set(key, value);
                string expirationTime = DateTime.UtcNow.Add(expiration).ToString("o"); // ISO 8601 format
                Set(key + "_ExpireTime", expirationTime);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error setting expiration for key {key}: {ex.Message}");
            }
        }

        public static string GetWithExpiration(string key, string defaultValue = "")
        {
            try
            {
                string expireKey = key + "_ExpireTime";
                if (PlayerPrefs.HasKey(expireKey))
                {
                    DateTime expireTime = DateTime.Parse(Get<string>(expireKey));
                    if (DateTime.UtcNow > expireTime)
                    {
                        Delete(key);
                        Delete(expireKey);
                        return defaultValue;
                    }
                }
                return Get(key, defaultValue);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error checking expiration for key {key}: {ex.Message}");
                return defaultValue;
            }
        }

        // === Simple Encryption Feature ===
        public static void SetEncryptedString(string key, string value)
        {
            try
            {
                string encryptedValue = Encrypt(value);
                Set(key, encryptedValue);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encrypting string for key {key}: {ex.Message}");
            }
        }

        public static string GetEncryptedString(string key, string defaultValue = "")
        {
            try
            {
                string encryptedValue = Get(key, "");
                return !string.IsNullOrEmpty(encryptedValue) ? Decrypt(encryptedValue) : defaultValue;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decrypting string for key {key}: {ex.Message}");
                return defaultValue;
            }
        }

        private static string Encrypt(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        private static string Decrypt(string value)
        {
            byte[] bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        // === Data Compression ===
        private static string Compress(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(bytes, 0, bytes.Length);
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        private static string Decompress(string compressedData)
        {
            byte[] bytes = Convert.FromBase64String(compressedData);
            using (var memoryStream = new MemoryStream(bytes))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gzipStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public static void SetCompressed(string key, string value)
        {
            string compressedValue = Compress(value);
            Set(key, compressedValue);
        }

        public static string GetCompressed(string key, string defaultValue = "")
        {
            string compressedValue = Get(key, "");
            return !string.IsNullOrEmpty(compressedValue) ? Decompress(compressedValue) : defaultValue;
        }

        // === Versioning ===
        public static void SetVersion(int version)
        {
            Set(VersionKey, version);
        }

        public static int GetVersion()
        {
            return Get(VersionKey, 1); // Default version is 1
        }

        public static void MigrateData(int oldVersion, int newVersion)
        {
            // Implement migration logic here
            if (oldVersion < newVersion)
            {
                // Example: Migrate data from version 1 to version 2
                if (oldVersion == 1 && newVersion == 2)
                {
                    // Migration logic
                }
            }
        }

        // === Cloud Sync ===
        public static void SyncToCloud()
        {
            try
            {
                // Serialize all PlayerPrefs data to JSON
                var allData = new Dictionary<string, object>();
                foreach (var key in PlayerPrefs.GetString("keys").Split(','))
                {
                    allData[key] = Get<object>(key);
                }
                string jsonData = JsonUtility.ToJson(allData);

                // Upload data to cloud
                CloudService.UploadData(jsonData);
                Debug.Log("Data synced to cloud.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error syncing data to cloud: {ex.Message}");
            }
        }

        public static void SyncFromCloud()
        {
            try
            {
                // Download data from cloud
                string jsonData = CloudService.DownloadData();

                // Deserialize JSON data to dictionary
                var allData = JsonUtility.FromJson<Dictionary<string, object>>(jsonData);

                // Set all data to PlayerPrefs
                foreach (var pair in allData)
                {
                    Set(pair.Key, pair.Value);
                }
                PlayerPrefs.Save();
                Debug.Log("Data synced from cloud.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error syncing data from cloud: {ex.Message}");
            }
        }

        [Serializable]
        private class SerializableObject
        {
            public object Value;

            public SerializableObject(object value)
            {
                Value = value;
            }
        }
    }

// Hypothetical Cloud Service API
    public static class CloudService
    {
        public static void UploadData(string data)
        {
            // Implement cloud upload logic here
            Debug.Log("Uploading data to cloud...");
        }

        public static string DownloadData()
        {
            // Implement cloud download logic here
            Debug.Log("Downloading data from cloud...");
            return "{}"; // Return empty JSON for example
        }
    }
}