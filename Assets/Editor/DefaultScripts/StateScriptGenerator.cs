#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class StateScriptGenerator : EditorWindow
{
    private string scriptName = "NewState";
    private string folderPath = "Assets/_Game/Scripts/Manager/Core/States";

    [MenuItem("Tools/MainraFramework/State Script Generator")]
    public static void ShowWindow()
    {
        GetWindow<StateScriptGenerator>("Script Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate New State Script", EditorStyles.boldLabel);

        // Input for script name
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

        GUILayout.Label($"Selected Folder: {folderPath}");

        // Informational Help Box
        EditorGUILayout.HelpBox("The script will be generated in the selected folder with the specified name. Make sure the script name is valid.", MessageType.Info);

        // Generate script button
        if (GUILayout.Button("Generate Script"))
        {
            GenerateScript();
        }
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
    public class {scriptName}State : IGameState
    {{
        public void EnterState()
        {{
            Debug.Log(""Entering {scriptName} State"");
            // Logika untuk memasuki state
        }}

        public void UpdateState()
        {{
            // Logika Update {scriptName}
        }}

        public void ExitState()
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
    }
}
#endif
