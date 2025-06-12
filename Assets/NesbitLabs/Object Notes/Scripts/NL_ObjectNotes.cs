using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class NL_ObjectNotes : MonoBehaviour
{
    public string noteTitle;

    [TextArea(3, 10)]
    public string noteText;

    public List<NL_ToDoItem> toDoList = new();

    public NL_NoteColorTag colorTag = NL_NoteColorTag.None;

    public bool showSceneIcon = true; // Used by Gizmo drawer
}

[Serializable]
public class NL_ToDoItem
{
    public string text = "New Task";
    public bool isDone = false;
}

public enum NL_NoteColorTag
{
    None,
    Red,
    Yellow,
    Green,
    Gray
}
