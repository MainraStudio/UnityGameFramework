#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        SyncWithGeneratedScript(); // Sync with the generated script
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

        // Sync with generated script button
        if (GUILayout.Button("Sync with Generated Script"))
        {
            SyncWithGeneratedScript();
        }

        // Backup option
        if (GUILayout.Button("Create Backup"))
        {
            CreateBackup();
        }

        // Import JSON button
        if (GUILayout.Button("Import JSON"))
        {
            ImportJson();
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
                bool isMatch = !string.IsNullOrEmpty(searchFilter) && category.Items[i].ToLower().Contains(searchFilter.ToLower());
                GUIStyle itemStyle = new GUIStyle(EditorStyles.textField);
                if (isMatch)
                {
                    itemStyle.normal.textColor = Color.yellow; // Highlight matching items
                }

                GUILayout.BeginHorizontal();
                category.Items[i] = EditorGUILayout.TextField(category.Items[i], itemStyle);
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    category.Items.RemoveAt(i);
                    isDirty = true;  // Data telah diubah
                }
                GUILayout.EndHorizontal();
            }

            // Add new item to the category
            if (GUILayout.Button($"Add Item to {category.Name}"))
            {
                category.Items.Add(string.Empty);
                isDirty = true;  // Data telah diubah
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
                isDirty = false; // Data sudah dimuat, tidak ada perubahan
            }
            catch
            {
                Debug.LogError("Failed to parse JSON file.");
                parameterData = new ParameterData();
                isDirty = true; // Menandakan bahwa data belum dimuat dengan benar
            }
        }
        else
        {
            parameterData = new ParameterData();
            isDirty = true; // Menandakan bahwa data kosong dan perlu disimpan
        }
    }

    // Save parameter data to JSON
    private void SaveData()
    {
        if (isDirty)
        {
            try
            {
                string json = JsonUtility.ToJson(parameterData, true);
                File.WriteAllText(JsonFilePath, json);
                AssetDatabase.Refresh();
                Debug.Log("Data saved successfully.");
                isDirty = false;
            }
            catch
            {
                Debug.LogError("Failed to save data to JSON.");
            }
        }
        else
        {
            Debug.Log("No changes to save.");
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

    // Sync data with the generated script
    private void SyncWithGeneratedScript()
    {
        if (!File.Exists(ScriptFilePath))
        {
            Debug.LogWarning("Generated script not found. Sync skipped.");
            return;
        }

        try
        {
            string scriptContent = File.ReadAllText(ScriptFilePath);
            // Regex to match categories (e.g., "public static class Tags")
            var categoryRegex = new Regex(@"public\s+static\s+class\s+(\w+)");
            // Regex to match items within a category (e.g., "public const string PLAYER = "Player";")
            var itemRegex = new Regex(@"public\s+const\s+string\s+\w+\s+=\s+""(.+?)"";");

            var matches = categoryRegex.Matches(scriptContent);
            var categories = new List<ParameterCategory>();

            foreach (Match match in matches)
            {
                string categoryName = match.Groups[1].Value;
                var categoryItems = new List<string>();

                int startIndex = scriptContent.IndexOf($"public static class {categoryName}", System.StringComparison.Ordinal);
                int endIndex = scriptContent.IndexOf("}", startIndex, System.StringComparison.Ordinal);
                string categoryBlock = scriptContent.Substring(startIndex, endIndex - startIndex);

                foreach (Match itemMatch in itemRegex.Matches(categoryBlock))
                {
                    categoryItems.Add(itemMatch.Groups[1].Value);
                }

                categories.Add(new ParameterCategory
                {
                    Name = categoryName,
                    Items = categoryItems
                });
            }

            foreach (var scriptCategory in categories)
            {
                var existingCategory = parameterData.Categories.FirstOrDefault(c => c.Name == scriptCategory.Name);
                if (existingCategory == null)
                {
                    parameterData.Categories.Add(scriptCategory);
                }
                else
                {
                    foreach (var item in scriptCategory.Items)
                    {
                        if (!existingCategory.Items.Contains(item))
                        {
                            existingCategory.Items.Add(item);
                        }
                    }
                }
            }

            Debug.Log("Data successfully synced with the generated script.");
            isDirty = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error syncing with generated script: {ex.Message}");
        }
    }

    // Import JSON file
    private void ImportJson()
    {
        string path = EditorUtility.OpenFilePanel("Import Parameter Data", "Assets", "json");
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                parameterData = JsonUtility.FromJson<ParameterData>(json);
                isDirty = true;
                Debug.Log("Data imported successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to import JSON: {ex.Message}");
            }
        }
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
