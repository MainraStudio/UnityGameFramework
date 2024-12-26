using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AudioClipLibrary))]
public class AudioClipLibraryEditor : Editor
{
    private SerializedProperty bgmEntries;
    private ReorderableList bgmList;

    private string searchQuery = ""; // Untuk fitur pencarian
    private SerializedProperty sfxEntries;
    private ReorderableList sfxList;

    private void OnEnable()
    {
        bgmEntries = serializedObject.FindProperty("bgmEntries");
        sfxEntries = serializedObject.FindProperty("sfxEntries");

        // Setup ReorderableList untuk BGM
        bgmList = new ReorderableList(serializedObject, bgmEntries, true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Background Music (BGM)"),
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = bgmEntries.GetArrayElementAtIndex(index);
                DrawAudioEntry(rect, element);
            }
        };

        // Setup ReorderableList untuk SFX
        sfxList = new ReorderableList(serializedObject, sfxEntries, true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Sound Effects (SFX)"),
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = sfxEntries.GetArrayElementAtIndex(index);
                DrawAudioEntry(rect, element);
            }
        };
    }

    private void DrawAudioEntry(Rect rect, SerializedProperty element)
    {
        var halfWidth = rect.width / 2;

        // Ambil property key dan clip dari AudioEntry
        var key = element.FindPropertyRelative("key");
        var clip = element.FindPropertyRelative("clip");

        // Field untuk key
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
            key, GUIContent.none
        );

        // Field untuk clip
        EditorGUI.PropertyField(
            new Rect(rect.x + halfWidth + 5, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
            clip, GUIContent.none
        );
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Field pencarian
        EditorGUILayout.LabelField("Search Audio Clips", EditorStyles.boldLabel);
        searchQuery = EditorGUILayout.TextField("Search Key", searchQuery);

        EditorGUILayout.Space();

        // Jika tidak ada query pencarian, tampilkan seluruh ReorderableList
        if (string.IsNullOrEmpty(searchQuery))
        {
            bgmList.DoLayoutList();
            EditorGUILayout.Space();
            sfxList.DoLayoutList();
        }
        else
        {
            // Tampilkan hasil pencarian
            ShowFilteredList(searchQuery);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowFilteredList(string query)
    {
        // Filter dan tampilkan hanya yang sesuai dengan query pencarian
        EditorGUILayout.LabelField("Filtered BGM", EditorStyles.boldLabel);
        ShowFilteredEntries(query, bgmEntries);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Filtered SFX", EditorStyles.boldLabel);
        ShowFilteredEntries(query, sfxEntries);
    }

    private void ShowFilteredEntries(string query, SerializedProperty entries)
    {
        for (var i = 0; i < entries.arraySize; i++)
        {
            var element = entries.GetArrayElementAtIndex(i);
            var key = element.FindPropertyRelative("key");

            // Jika key cocok dengan query, tampilkan
            if (key.stringValue.ToLower().Contains(query.ToLower()))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(element, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}