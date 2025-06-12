using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NL_SceneGizmoDrawer
{
    static Texture2D noteIcon;

    static NL_SceneGizmoDrawer()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        noteIcon = EditorGUIUtility.IconContent("console.infoicon").image as Texture2D;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!noteIcon) return;

        foreach (var note in Object.FindObjectsOfType<NL_ObjectNotes>())
        {
            if (!note.showSceneIcon || note == null || !note.gameObject.activeInHierarchy)
                continue;

            Vector3 worldPos = note.transform.position;
            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(worldPos);
            Handles.BeginGUI();

            GUI.color = GetColor(note.colorTag);
            GUI.DrawTexture(new Rect(guiPoint.x - 8, guiPoint.y - 8, 16, 16), noteIcon);
            GUI.color = Color.white;

            Handles.EndGUI();
        }
    }

    private static Color GetColor(NL_NoteColorTag tag)
    {
        return tag switch
        {
            NL_NoteColorTag.Red => Color.red,
            NL_NoteColorTag.Yellow => Color.yellow,
            NL_NoteColorTag.Green => Color.green,
            NL_NoteColorTag.Gray => Color.gray,
            _ => Color.white
        };
    }
}
