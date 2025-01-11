using UnityEngine;
using System;
using System.Collections.Generic;

public static class PlayerPrefsManager
{
    // === Fungsi Dasar dengan Fluent API ===
    public static void Set(string key, object value)
    {
        try
        {
            switch (value)
            {
                case int intValue:
                    PlayerPrefs.SetInt(key, intValue);
                    break;
                case float floatValue:
                    PlayerPrefs.SetFloat(key, floatValue);
                    break;
                case string stringValue:
                    PlayerPrefs.SetString(key, stringValue);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported type for key {key}: {value.GetType()}");
            }

            PlayerPrefs.Save();
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
            if (typeof(T) == typeof(int))
                return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
            if (typeof(T) == typeof(float))
                return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
            if (typeof(T) == typeof(string))
                return (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);

            throw new InvalidOperationException($"Unsupported type for key {key}");
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
        }
    }

    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    // === Fungsi JSON ===
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

    // === Fitur Kedaluwarsa ===
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

    // === Fitur Enkripsi Sederhana ===
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
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }

    private static string Decrypt(string value)
    {
        byte[] bytes = Convert.FromBase64String(value);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
