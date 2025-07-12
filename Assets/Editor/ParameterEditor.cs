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
    private const string ScriptFilePath = "Assets/_Game/Scripts/Utility/Unity/Parameter.cs";

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

    // Autosave settings
    private bool autoSaveEnabled = true;
    private double lastChangeTime;
    private const double autoSaveDelay = 30.0; // Autosave delay in seconds
    private const string AutoSavePrefsKey = "ParameterEditor_AutoSave";

    [MenuItem("Tools/MainraFramework/Parameter Editor")]
    public static void ShowWindow()
    {
        GetWindow<ParameterEditor>("Parameter Editor");
    }

    private void OnEnable()
    {
        LoadData();
        AddDefaultCategories();
        SyncWithGeneratedScript();

        // Load autosave preference
        autoSaveEnabled = EditorPrefs.GetBool(AutoSavePrefsKey, true);

        // Start the autosave update loop
        EditorApplication.update += AutoSaveUpdate;
    }

    private void OnDisable()
    {
        // Simpan perubahan sebelum keluar dari window
        if (isDirty)
        {
            SaveData();
            GenerateScript();
        }

        // Clean up update loop
        EditorApplication.update -= AutoSaveUpdate;
    }

    private void AutoSaveUpdate()
    {
        if (!autoSaveEnabled || !isDirty)
            return;

        if (EditorApplication.timeSinceStartup - lastChangeTime >= autoSaveDelay)
        {
            SaveData();
            GenerateScript();
        }
    }

    private void MarkDirty()
    {
        isDirty = true;
        lastChangeTime = EditorApplication.timeSinceStartup;
    }

    private void OnGUI()
    {
        GUILayout.Label("Edit Parameters", EditorStyles.boldLabel);

        // Autosave toggle
        EditorGUI.BeginChangeCheck();
        autoSaveEnabled = EditorGUILayout.ToggleLeft("Enable Autosave", autoSaveEnabled);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetBool(AutoSavePrefsKey, autoSaveEnabled);
        }

        if (autoSaveEnabled && isDirty)
        {
            double timeUntilSave = autoSaveDelay - (EditorApplication.timeSinceStartup - lastChangeTime);
            if (timeUntilSave > 0)
            {
                EditorGUILayout.HelpBox($"Autosaving in {timeUntilSave:F0} seconds...", MessageType.Info);
            }
        }

        if (parameterData == null)
        {
            EditorGUILayout.HelpBox("Failed to load parameter data.", MessageType.Error);
            if (GUILayout.Button("Create New Data"))
            {
                parameterData = new ParameterData();
                AddDefaultCategories();
                MarkDirty();
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
            MarkDirty();
        }

        GUILayout.Space(20);

        // Manual save button
        using (new EditorGUI.DisabledGroupScope(!isDirty))
        {
            if (GUILayout.Button("Save & Generate Script"))
            {
                SaveData();
                GenerateScript();
            }
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
            EditorGUI.BeginChangeCheck();
            category.Name = EditorGUILayout.TextField("Category Name", category.Name);
            if (EditorGUI.EndChangeCheck())
            {
                MarkDirty();
            }

            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                parameterData.Categories.RemoveAt(index);
                categoryVisibility.Remove(index); // Remove visibility state for the deleted category
                MarkDirty();
                return;
            }
            GUILayout.EndHorizontal();

            // Items in the category
            for (int i = 0; i < category.Items.Count; i++)
            {
                bool isMatch = !string.IsNullOrEmpty(searchFilter) &&
                             category.Items[i].ToLower().Contains(searchFilter.ToLower());
                GUIStyle itemStyle = new GUIStyle(EditorStyles.textField);
                if (isMatch)
                {
                    itemStyle.normal.textColor = Color.yellow;
                }

                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                category.Items[i] = EditorGUILayout.TextField(category.Items[i], itemStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    MarkDirty();
                }

                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    category.Items.RemoveAt(i);
                    MarkDirty();
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button($"Add Item to {category.Name}"))
            {
                category.Items.Add(string.Empty);
                MarkDirty();
            }

            GUILayout.EndVertical();
            GUILayout.Space(10);
        }
    }

    private void AddNewCategory()
    {
        parameterData.Categories.Add(new ParameterCategory
        {
            Name = "NewCategory",
            Items = new List<string>()
        });
        MarkDirty();
    }

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

            MarkDirty();
        }
    }

    private void LoadData()
    {
        if (File.Exists(JsonFilePath))
        {
            try
            {
                string json = File.ReadAllText(JsonFilePath);
                parameterData = JsonUtility.FromJson<ParameterData>(json);
                isDirty = false;
            }
            catch
            {
                Debug.LogError("Failed to parse JSON file.");
                parameterData = new ParameterData();
                isDirty = false; // No changes yet
            }
        }
        else
        {
            parameterData = new ParameterData();
            isDirty = false; // No changes yet
        }
    }

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
    }

    private void GenerateScript()
    {
        if (parameterData == null || parameterData.Categories.Count == 0)
        {
            Debug.LogWarning("No data to generate script.");
            return;
        }

        try
        {
            using (StreamWriter writer = new StreamWriter(ScriptFilePath))
            {
                writer.WriteLine("namespace MainraFramework.Parameter");
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

    private void CreateBackup()
    {
        string backupPath = JsonFilePath.Replace(".json", $"_Backup_{System.DateTime.Now:yyyyMMddHHmmss}.json");
        File.Copy(JsonFilePath, backupPath, true);
        Debug.Log($"Backup created at {backupPath}");
    }

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
            var categoryRegex = new Regex(@"public\s+static\s+class\s+(\w+)");
            var itemRegex = new Regex(@"public\s+const\s+string\s+(\w+)\s+=\s+""(.+?)"";");

            var matches = categoryRegex.Matches(scriptContent);
            var scriptCategories = new List<ParameterCategory>();

            // Extract all categories and items from the script
            foreach (Match match in matches)
            {
                string categoryName = match.Groups[1].Value;
                if (categoryName != "Parameter")
                {
                    var categoryItems = new List<string>();

                    int startIndex = scriptContent.IndexOf($"public static class {categoryName}", System.StringComparison.Ordinal);
                    int endIndex = scriptContent.IndexOf("}", startIndex, System.StringComparison.Ordinal);
                    string categoryBlock = scriptContent.Substring(startIndex, endIndex - startIndex);

                    foreach (Match itemMatch in itemRegex.Matches(categoryBlock))
                    {
                        categoryItems.Add(itemMatch.Groups[2].Value);
                    }

                    scriptCategories.Add(new ParameterCategory
                    {
                        Name = categoryName,
                        Items = categoryItems
                    });
                }
            }

            // Update existing categories with items from script
            var updatedCategories = new List<ParameterCategory>();
            foreach (var scriptCategory in scriptCategories)
            {
                var existingCategory = parameterData.Categories.FirstOrDefault(c => c.Name == scriptCategory.Name);
                if (existingCategory != null)
                {
                    existingCategory.Items = new List<string>(scriptCategory.Items);
                    updatedCategories.Add(existingCategory);
                }
                else
                {
                    updatedCategories.Add(scriptCategory);
                }
            }

            // Remove categories that are not in the script
            parameterData.Categories = updatedCategories;

            // Update category visibility
            categoryVisibility.Clear();
            for (int i = 0; i < parameterData.Categories.Count; i++)
            {
                categoryVisibility[i] = true;
            }

            Debug.Log("Data successfully synced with the generated script.");
            MarkDirty();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error syncing with generated script: {ex.Message}");
        }
    }

    private void ImportJson()
    {
        string path = EditorUtility.OpenFilePanel("Import Parameter Data", "Assets", "json");
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                parameterData = JsonUtility.FromJson<ParameterData>(json);
                MarkDirty();
                Debug.Log("Data imported successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to import JSON: {ex.Message}");
            }
        }
    }

    private string GetConstantName(string input)
    {
        return input.ToUpper().Replace(" ", "_").Replace("-", "_");
    }
}

[System.Serializable]
public class ParameterData
{
    public List<ParameterCategory> Categories = new List<ParameterCategory>();
}

[System.Serializable]
public class ParameterCategory
{
    public string Name;
    public List<string> Items = new List<string>();
}
#endif