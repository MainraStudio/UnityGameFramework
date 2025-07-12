using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NL_ObjectNotes_Readme))]
public class NL_ObjectNotes_ReadmeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var readme = (NL_ObjectNotes_Readme)target;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };

        GUIStyle versionStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleCenter
        };

        GUILayout.Space(10);
        GUILayout.Label("📘 Nesbit Labs - Object Notes", titleStyle);
        GUILayout.Label("Version: " + readme.version, versionStyle);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(readme.thankYouMessage, MessageType.Info);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Changelog", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(readme.changeLog, GUILayout.MinHeight(100));

    }
}
