#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class StateScriptGenerator : EditorWindow
{
    private string scriptName = "NewState";
    private string folderPath = "Assets/_Game/Scripts/Manager/Core/States";
    private string[] generatedScripts = new string[0];

    [MenuItem("Tools/MainraFramework/State Script Generator")]
    public static void ShowWindow()
    {
        GetWindow<StateScriptGenerator>("State Script Generator");
    }

    private void OnEnable()
    {
        RefreshGeneratedScripts();
    }

    private Vector2 scrollPosition;

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("State Script Generator", EditorStyles.boldLabel);

            // Input untuk script name
            EditorGUILayout.HelpBox("Enter the script name without 'State' at the end.", MessageType.Info);
            scriptName = EditorGUILayout.TextField("Script Name", scriptName);

            // Select folder
            if (GUILayout.Button("Select Folder"))
            {
                string path = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    folderPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
            }

            EditorGUILayout.LabelField($"Selected Folder: {folderPath}");

            // Informasi Help Box
            EditorGUILayout.HelpBox("The script will be generated in the selected folder with the specified name. Make sure the script name is valid.", MessageType.Info);

            // Generate script button
            if (GUILayout.Button("Generate Script"))
            {
                GenerateScript();
            }

            EditorGUILayout.Space();

            // Daftar script yang sudah digenerate
            EditorGUILayout.LabelField("Generated Scripts", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                foreach (string script in generatedScripts)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(script);
                        if (GUILayout.Button("Open"))
                        {
                            OpenScriptInEditor(script);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();

            // Refresh button
            if (GUILayout.Button("Refresh"))
            {
                RefreshGeneratedScripts();
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void GenerateScript()
    {
        // Validate script name
        if (string.IsNullOrEmpty(scriptName))
        {
            Debug.LogError("Script name cannot be empty!");
            return;
        }

        if (!char.IsLetter(scriptName[0]) || scriptName.Any(char.IsWhiteSpace))
        {
            Debug.LogError("Script name must not contain spaces and must start with a letter.");
            return;
        }

        // Ensure folder exists or create it
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"Folder '{folderPath}' was created.");
        }

        string scriptPath = Path.Combine(folderPath, scriptName + "State.cs");

        // Check if file exists
        if (File.Exists(scriptPath))
        {
            if (!EditorUtility.DisplayDialog("Overwrite File", $"A script named '{scriptName}State.cs' already exists. Overwrite?", "Yes", "No"))
            {
                return;
            }
        }

        // Generate script template
        string scriptTemplate = $@"using UnityEngine;

namespace MainraFramework.States
{{
    public class {scriptName}State : GameState
    {{
        public override void EnterState()
        {{
            Debug.Log(""Entering {scriptName} State"");
            // Logika untuk memasuki state
        }}

        public override void UpdateState()
        {{
            // Logika Update {scriptName}
        }}

        public override void ExitState()
        {{
            Debug.Log(""Exiting {scriptName} State"");
            // Logika keluar dari {scriptName}
        }}
    }}
}}";

        // Write the script to the file
        File.WriteAllText(scriptPath, scriptTemplate);

        // Refresh the AssetDatabase
        AssetDatabase.Refresh();

        Debug.Log($"Script '{scriptName}State.cs' created at {folderPath}");

        RefreshGeneratedScripts();
    }

    private void RefreshGeneratedScripts()
    {
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath, "*.cs");
            generatedScripts = files.Select(file => Path.GetFileName(file)).ToArray();

            // Filter script yang sudah di generate
            generatedScripts = generatedScripts.Where(script => script.EndsWith("State.cs")).ToArray();
        }
        else
        {
            generatedScripts = new string[0];
        }
    }

    private void OpenScriptInEditor(string scriptName)
    {
        string scriptPath = Path.Combine(folderPath, scriptName);
        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(scriptPath, typeof(TextAsset)));
    }
}
#endif