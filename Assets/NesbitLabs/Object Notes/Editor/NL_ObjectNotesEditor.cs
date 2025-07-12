using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(NL_ObjectNotes))]
public class NL_ObjectNotesEditor : Editor
{
    SerializedProperty noteTitle;
    SerializedProperty noteText;
    SerializedProperty toDoList;
    SerializedProperty colorTag;
    SerializedProperty showSceneIcon;

    ReorderableList todoReorderableList;

    void OnEnable()
    {
        noteTitle = serializedObject.FindProperty("noteTitle");
        noteText = serializedObject.FindProperty("noteText");
        toDoList = serializedObject.FindProperty("toDoList");
        colorTag = serializedObject.FindProperty("colorTag");
        showSceneIcon = serializedObject.FindProperty("showSceneIcon");

        todoReorderableList = new ReorderableList(serializedObject, toDoList, true, true, true, true);
        todoReorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "To-Do List");

        todoReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var item = toDoList.GetArrayElementAtIndex(index);
            var isDone = item.FindPropertyRelative("isDone");
            var text = item.FindPropertyRelative("text");

            float checkboxWidth = 18f;
            Rect checkboxRect = new Rect(rect.x, rect.y + 2, checkboxWidth, EditorGUIUtility.singleLineHeight);
            Rect textRect = new Rect(rect.x + checkboxWidth + 5, rect.y + 2, rect.width - checkboxWidth - 5, EditorGUIUtility.singleLineHeight);

            isDone.boolValue = EditorGUI.Toggle(checkboxRect, isDone.boolValue);
            text.stringValue = EditorGUI.TextField(textRect, text.stringValue);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("📝 Object Notes", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(noteTitle, new GUIContent("Title"));
        EditorGUILayout.PropertyField(noteText, new GUIContent("Notes"));

        EditorGUILayout.Space();
        todoReorderableList.DoLayoutList();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(colorTag, new GUIContent("Color Tag"));
        EditorGUILayout.PropertyField(showSceneIcon, new GUIContent("Show Scene Icon"));

        serializedObject.ApplyModifiedProperties();
    }
}
