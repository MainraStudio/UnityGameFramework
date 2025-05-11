#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.IO;

namespace DebugToolsPlus.Editor
{
    public class ReadDumpWindow : EditorWindow
    {
        [MenuItem("Tools/DebugToolsPlus/Read Dump")]
        public static void OpenWindow()
        {
            GetWindow<ReadDumpWindow>("Read Dump");
        }

        string day;
        string hour;
        string minute;
        string second;
        string title;

        string[] columns = { "DATE", "NAME" };
        int columnIndex = 0;

        Vector2 scrollPos;

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Please input the date time of the dump file.");
            EditorGUILayout.HelpBox("You can see the date time in the title of the dump file.", MessageType.Info);

            Object folder = AssetDatabase.LoadAssetAtPath<Object>(DebugTool.DUMP_PATH);

            if (folder != null)
            {
                if (GUILayout.Button("Open Dumps Folder"))
                {
                    EditorGUIUtility.PingObject(folder);
                    Selection.activeObject = folder;
                }

                EditorGUILayout.Space();

                columnIndex = GUILayout.Toolbar(columnIndex, columns, EditorStyles.toolbarButton);

                if (columnIndex == 0)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    day = EditorGUILayout.TextField(new GUIContent("Day"), day);
                    hour = EditorGUILayout.TextField(new GUIContent("Hour"), hour);
                    minute = EditorGUILayout.TextField(new GUIContent("Minute"), minute);
                    second = EditorGUILayout.TextField(new GUIContent("Second"), second);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                    EditorGUILayout.LabelField("Dump File", EditorStyles.boldLabel);
                    string[] lines = ReadDumpText();
                    if (lines != null)
                    {
                        foreach (string line in lines)
                        {
                            EditorGUILayout.LabelField(line);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField(GetPath());
                        EditorGUILayout.LabelField("File not Found.");
                    }

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    title = EditorGUILayout.TextField(new GUIContent("Title"), title);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                    EditorGUILayout.LabelField("Dump File", EditorStyles.boldLabel);
                    string[] lines = ReadDumpText(title);
                    if (lines != null)
                    {
                        foreach (string line in lines)
                        {
                            EditorGUILayout.LabelField(line);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField(GetPath(title));
                        EditorGUILayout.LabelField("File not Found.");
                    }

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.EndVertical();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Dumps folder doesnt exists.");
            }

            EditorGUILayout.EndVertical();
        }

        string GetPath()
        {
            return $"{DebugTool.DUMP_PATH}/{DebugTool.DUMP_NAME}{day}_{hour}_{minute}_{second}.txt";
        }

        string GetPath(string title)
        {
            return $"{DebugTool.DUMP_PATH}/{title}.txt";
        }

        string[] ReadDumpText()
        {
            if (File.Exists(GetPath()))
            {
                return File.ReadAllLines(GetPath());
            }

            return null;
        }

        string[] ReadDumpText(string title)
        {
            if (File.Exists(GetPath(title)))
            {
                return File.ReadAllLines(GetPath(title));
            }
            return null;
        }
    }
}

#endif
