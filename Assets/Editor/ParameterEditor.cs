#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ParameterEditor : EditorWindow
{
    // Constants for file paths
    private const string JsonFilePath = "Assets/Editor/ParameterData/ParameterData.json";
    private const string ScriptFilePath = "Assets/_Game/Scripts/Manager/Core/Parameter.cs";

    // Core data structure
    private ParameterData parameterData;

    // UI scrolling
    private Vector2 scrollPosition;

    // Search filter for categories/items
    private string searchFilter = "";

    // Undo/Redo management
    private bool isDirty = false;

    // Track visibility of categories (collapsed/expanded)
    private Dictionary<int, bool> categoryVisibility = new Dictionary<int, bool>();

    [MenuItem("Tools/MainraFramework/Parameter Editor")]
    public static void ShowWindow()
    {
        GetWindow<ParameterEditor>("Parameter Editor");
    }

    // Load data when the editor window is opened
    private void OnEnable()
    {
        LoadData();
        AddDefaultCategories();
    }

    // Main GUI layout
    private void OnGUI()
    {
        GUILayout.Label("Edit Parameters", EditorStyles.boldLabel);

        if (parameterData == null)
        {
            EditorGUILayout.HelpBox("Failed to load parameter data.", MessageType.Error);
            if (GUILayout.Button("Create New Data"))
            {
                parameterData = new ParameterData();
                AddDefaultCategories(); // Add default categories if the file is new
                isDirty = true;
            }
            return;
        }

        // Search functionality
        searchFilter = EditorGUILayout.TextField("Search", searchFilter);

        // Scrollable list for categories
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        var filteredCategories = parameterData.Categories
            .Where(c => c.Name.ToLower().Contains(searchFilter.ToLower()) || 
                        c.Items.Any(item => item.ToLower().Contains(searchFilter.ToLower())))
            .ToList();

        for (int i = 0; i < filteredCategories.Count; i++)
        {
            DrawCategory(filteredCategories[i], i);
        }
        GUILayout.EndScrollView();

        // Add new category button
        if (GUILayout.Button("Add New Category"))
        {
            AddNewCategory();
            isDirty = true;
        }

        GUILayout.Space(20);

        // Save and generate script button
        if (GUILayout.Button("Save & Generate Script"))
        {
            SaveData();
            GenerateScript();
            isDirty = false;
        }

        // Backup option
        if (GUILayout.Button("Create Backup"))
        {
            CreateBackup();
        }
    }

    // Draw a single category with its items
    private void DrawCategory(ParameterCategory category, int index)
    {
        if (!categoryVisibility.ContainsKey(index))
            categoryVisibility[index] = true;

        categoryVisibility[index] = EditorGUILayout.Foldout(categoryVisibility[index], category.Name);
        if (categoryVisibility[index])
        {
            GUILayout.BeginVertical("box");

            // Category header
            GUILayout.BeginHorizontal();
            category.Name = EditorGUILayout.TextField("Category Name", category.Name);

            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                parameterData.Categories.RemoveAt(index);
                isDirty = true;
                return;
            }
            GUILayout.EndHorizontal();

            // Items in the category
            for (int i = 0; i < category.Items.Count; i++)
            {
                // Only highlight matching parameters if search is not empty
                bool isMatch = !string.IsNullOrEmpty(searchFilter) && category.Items[i].ToLower().Contains(searchFilter.ToLower());
                GUIStyle itemStyle = new GUIStyle(EditorStyles.textField);
                if (isMatch)
                {
                    itemStyle.normal.textColor = Color.yellow; // Highlight color
                }

                GUILayout.BeginHorizontal();
                category.Items[i] = EditorGUILayout.TextField(category.Items[i], itemStyle);
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    category.Items.RemoveAt(i);
                    isDirty = true;
                }
                GUILayout.EndHorizontal();
            }

            // Add new item to the category
            if (GUILayout.Button($"Add Item to {category.Name}"))
            {
                category.Items.Add(string.Empty);
                isDirty = true;
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);
        }
    }

    // Add a new category
    private void AddNewCategory()
    {
        parameterData.Categories.Add(new ParameterCategory
        {
            Name = "NewCategory",
            Items = new List<string>()
        });
    }

    // Add default categories if no data exists
    private void AddDefaultCategories()
    {
        if (parameterData.Categories.Count == 0)
        {
            parameterData.Categories.Add(new ParameterCategory
            {
                Name = "Tags",
                Items = new List<string> { "Player", "Enemy", "NPC", "Item", "Pickable", "Ground" }
            });

            parameterData.Categories.Add(new ParameterCategory
            {
                Name = "Scenes",
                Items = new List<string> { "MainMenu", "Gameplay", "GameOver", "Credits" }
            });

            parameterData.Categories.Add(new ParameterCategory
            {
                Name = "AnimParams",
                Items = new List<string> { "isRunning", "isJumping", "attackTrigger" }
            });

            parameterData.Categories.Add(new ParameterCategory
            {
                Name = "Layers",
                Items = new List<string> { "Ground", "Water", "PlayerLayer" }
            });

            parameterData.Categories.Add(new ParameterCategory
            {
                Name = "UI",
                Items = new List<string> { "StartButton", "PauseButton", "ScoreText", "HealthBar" }
            });

            parameterData.Categories.Add(new ParameterCategory
            {
                Name = "AudioClips",
                Items = new List<string> { "gameplay", "gameplay2", "ButtonClick", "VictorySound", "coin", "Win" }
            });

            isDirty = true;
        }
    }

    // Load parameter data from JSON
    private void LoadData()
    {
        if (File.Exists(JsonFilePath))
        {
            try
            {
                string json = File.ReadAllText(JsonFilePath);
                parameterData = JsonUtility.FromJson<ParameterData>(json);
            }
            catch
            {
                Debug.LogError("Failed to parse JSON file.");
                parameterData = new ParameterData();
            }
        }
        else
        {
            parameterData = new ParameterData();
        }
    }

    // Save parameter data to JSON
    private void SaveData()
    {
        try
        {
            string json = JsonUtility.ToJson(parameterData, true);
            File.WriteAllText(JsonFilePath, json);
            AssetDatabase.Refresh();
            Debug.Log("Data saved successfully.");
        }
        catch
        {
            Debug.LogError("Failed to save data to JSON.");
        }
    }

    // Generate the Parameter.cs script dynamically
    private void GenerateScript()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(ScriptFilePath))
            {
                writer.WriteLine("namespace MainraFramework");
                writer.WriteLine("{");
                writer.WriteLine("    public static class Parameter");
                writer.WriteLine("    {");

                foreach (var category in parameterData.Categories)
                {
                    if (!string.IsNullOrWhiteSpace(category.Name))
                    {
                        writer.WriteLine($"        public static class {category.Name}");
                        writer.WriteLine("        {");

                        foreach (var item in category.Items)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                string constantName = GetConstantName(item);
                                writer.WriteLine($"            public const string {constantName} = \"{item}\";");
                            }
                        }

                        writer.WriteLine("        }");
                    }
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            Debug.Log("Parameter script successfully generated.");
        }
        catch
        {
            Debug.LogError("Failed to generate Parameter script.");
        }
    }

    // Create a backup of the JSON data
    private void CreateBackup()
    {
        string backupPath = JsonFilePath.Replace(".json", $"_Backup_{System.DateTime.Now:yyyyMMddHHmmss}.json");
        File.Copy(JsonFilePath, backupPath, true);
        Debug.Log($"Backup created at {backupPath}");
    }

    // Helper: Convert a string to a constant-friendly format
    private string GetConstantName(string input)
    {
        return input.ToUpper().Replace(" ", "_").Replace("-", "_");
    }
}

// Data structure for parameter categories
[System.Serializable]
public class ParameterData
{
    public List<ParameterCategory> Categories = new List<ParameterCategory>();
}

// Data structure for individual categories
[System.Serializable]
public class ParameterCategory
{
    public string Name;
    public List<string> Items = new List<string>();
}
#endif
