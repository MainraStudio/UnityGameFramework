using UnityEditor;
using UnityEngine;

public class NL_NotesOverviewWindow : EditorWindow
{
    Vector2 scroll;

    [MenuItem("Window/Nesbit Labs/Object Notes Overview")]
    public static void ShowWindow()
    {
        GetWindow<NL_NotesOverviewWindow>("Object Notes Overview");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("🗒️ Scene Note Overview", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        NL_ObjectNotes[] notes = FindObjectsOfType<NL_ObjectNotes>();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        if (notes.Length == 0)
        {
            EditorGUILayout.HelpBox("No objects in the scene have notes attached.", MessageType.Info);
        }

        foreach (var note in notes)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(note.noteTitle != "" ? note.noteTitle : "(Untitled Note)", EditorStyles.boldLabel);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeGameObject = note.gameObject;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(note.noteText, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            foreach (var item in note.toDoList)
            {
                string prefix = item.isDone ? "✔ " : "☐ ";
                string task = item.isDone ? $"<color=grey><s>{item.text}</s></color>" : item.text;
                EditorGUILayout.LabelField(prefix + item.text);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();
    }
}
