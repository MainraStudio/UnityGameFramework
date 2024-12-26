using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipLibrary", menuName = "Audio/AudioClipLibrary")]
public class AudioClipLibrary : ScriptableObject
{
    // Kelas AudioEntry untuk menyimpan data audio
    [System.Serializable]
    public class AudioEntry
    {
        public string key; // Kunci unik (misalnya "Jump", "Explosion")
        public AudioClip clip; // AudioClip yang sesuai
    }

    [Header("BGM Clips")]
    public List<AudioEntry> bgmEntries = new List<AudioEntry>();  // Daftar audio BGM

    [Header("SFX Clips")]
    public List<AudioEntry> sfxEntries = new List<AudioEntry>();  // Daftar audio SFX

    private Dictionary<string, AudioClip> bgmDictionary;  // Dictionary untuk BGM
    private Dictionary<string, AudioClip> sfxDictionary;  // Dictionary untuk SFX

    private void OnEnable()
    {
        // Inisialisasi Dictionary untuk akses cepat
        bgmDictionary = new Dictionary<string, AudioClip>();
        sfxDictionary = new Dictionary<string, AudioClip>();

        // Isi Dictionary untuk BGM
        foreach (var entry in bgmEntries)
        {
            if (!bgmDictionary.ContainsKey(entry.key))
            {
                bgmDictionary.Add(entry.key, entry.clip);
            }
        }

        // Isi Dictionary untuk SFX
        foreach (var entry in sfxEntries)
        {
            if (!sfxDictionary.ContainsKey(entry.key))
            {
                sfxDictionary.Add(entry.key, entry.clip);
            }
        }
    }

    /// <summary>
    /// Mendapatkan AudioClip berdasarkan key dan jenisnya (BGM/SFX)
    /// </summary>
    /// <param name="key">Key untuk mencari AudioClip.</param>
    /// <param name="isBGM">Jika true, mencari di BGM, jika false mencari di SFX.</param>
    /// <returns>AudioClip yang ditemukan, atau null jika tidak ditemukan.</returns>
    public AudioClip GetAudioClip(string key, bool isBGM)
    {
        // Pilih dictionary berdasarkan jenis audio (BGM atau SFX)
        if (isBGM)
        {
            if (bgmDictionary.TryGetValue(key, out AudioClip clip))
            {
                return clip;
            }
        }
        else
        {
            if (sfxDictionary.TryGetValue(key, out AudioClip clip))
            {
                return clip;
            }
        }

        Debug.LogWarning($"AudioClip dengan key '{key}' tidak ditemukan!");
        return null;
    }
}
