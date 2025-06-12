using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NL_SelectReadmeOnLoad
{
    private const string readmeShownKey = "NL_ObjectNotes_ReadmeShown";

    static NL_SelectReadmeOnLoad()
    {
        if (!EditorPrefs.GetBool(readmeShownKey, false))
        {
            EditorApplication.update += ShowCPGReadmeOnce;
        }
    }

    private static void ShowCPGReadmeOnce()
    {
        EditorApplication.update -= ShowCPGReadmeOnce;

        string[] guids = AssetDatabase.FindAssets("t:NL_ObjectNotes_Readme");
        if (guids.Length == 0) return;

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Object readmeAsset = AssetDatabase.LoadAssetAtPath<Object>(path);

        if (readmeAsset != null)
        {
            Selection.activeObject = readmeAsset;
            EditorGUIUtility.PingObject(readmeAsset);
        }

        EditorPrefs.SetBool(readmeShownKey, true);
    }
}

