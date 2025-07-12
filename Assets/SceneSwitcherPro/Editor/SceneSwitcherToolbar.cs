#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using System.Linq;
using System.IO;

[InitializeOnLoad]
public static class SceneSwitcherToolbar
{
    private static string[] sceneNames = new string[0];
    private static int selectedIndex = 0;
    private static string lastActiveScene = "";
    private static VisualElement toolbarUI;

    private static float positionOffset = 180f; // Move closer to Play button
    private static float dropdownBoxHeight = 20f; // Dropdown button height

    private static bool fetchAllScenes
    {
        get => EditorPrefs.GetBool("SceneSwitcher_FetchAllScenes", false);
        set => EditorPrefs.SetBool("SceneSwitcher_FetchAllScenes", value);
    }

    static SceneSwitcherToolbar()
    {
        RefreshSceneList();
        SelectCurrentScene();


        Debug.Log(
            "<b><color=green>Thank you for using Scene Switcher Pro</color></b>\n" +
            "If you find this tool helpful, please consider leaving a review on the Asset Store."
        );

        // Hook into scene change events
        EditorSceneManager.activeSceneChangedInEditMode += (prev, current) => UpdateSceneSelection();
        EditorApplication.playModeStateChanged += OnPlayModeChanged;

        EditorApplication.delayCall += AddToolbarUI;
    }

    static void AddToolbarUI()
    {
        var toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        if (toolbarType == null) return;

        var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
        if (toolbars.Length == 0) return;

        var toolbar = toolbars[0];
        var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
        if (rootField == null) return;

        var root = rootField.GetValue(toolbar) as VisualElement;
        if (root == null) return;

        var leftContainer = root.Q("ToolbarZoneLeftAlign");
        if (leftContainer == null) return;

        // Remove old UI if it exists to prevent duplication
        if (toolbarUI != null)
        {
            leftContainer.Remove(toolbarUI);
        }

        toolbarUI = new IMGUIContainer(OnGUI);
        toolbarUI.style.marginLeft = positionOffset;

        leftContainer.Add(toolbarUI);
    }

    static void OnGUI()
    {
        CheckAndRefreshScenes();

        if (selectedIndex >= sceneNames.Length)
            selectedIndex = 0;

        bool isPlaying = EditorApplication.isPlaying; // Check if in Play Mode

        GUILayout.BeginHorizontal();

        // Fetch all scenes toggle button (Disabled in Play Mode)
        EditorGUI.BeginDisabledGroup(isPlaying);
        bool newFetchAllScenes = GUILayout.Toggle(fetchAllScenes, "All Scenes", "Button", GUILayout.Height(dropdownBoxHeight));
        if (newFetchAllScenes != fetchAllScenes)
        {
            fetchAllScenes = newFetchAllScenes;
            RefreshSceneList();
            SelectCurrentScene();
        }
        EditorGUI.EndDisabledGroup();

        // Scene dropdown with the currently selected scene displayed (Disabled in Play Mode)
        EditorGUI.BeginDisabledGroup(isPlaying);
        GUIStyle popupStyle = new GUIStyle(EditorStyles.popup)
        {
            fixedHeight = dropdownBoxHeight
        };

        int newIndex = EditorGUILayout.Popup(selectedIndex, sceneNames, popupStyle, GUILayout.Width(150), GUILayout.Height(dropdownBoxHeight));

        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            LoadScene(sceneNames[selectedIndex]);
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();
    }

    static void RefreshSceneList()
    {
        if (fetchAllScenes)
        {
            sceneNames = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .ToArray();
        }
        else
        {
            sceneNames = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                .ToArray();
        }
    }

    static void CheckAndRefreshScenes()
    {
        string[] currentScenes;
        if (fetchAllScenes)
        {
            currentScenes = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .ToArray();
        }
        else
        {
            currentScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                .ToArray();
        }

        if (!currentScenes.SequenceEqual(sceneNames))
        {
            sceneNames = currentScenes;
            SelectCurrentScene();
        }
    }

    static void SelectCurrentScene()
    {
        string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        int index = System.Array.IndexOf(sceneNames, currentScene);

        if (index != -1)
        {
            selectedIndex = index;
            lastActiveScene = currentScene;
        }
        else
        {
            // Append "(not in build index)" if the scene isn't listed
            string notInBuildName = currentScene + " (not in build index)";

            // Insert it at the beginning or replace first element
            sceneNames = new[] { notInBuildName }.Concat(sceneNames).ToArray();
            selectedIndex = 0;
            lastActiveScene = currentScene;
        }
    }

    static void UpdateSceneSelection()
    {
        string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        if (currentScene != lastActiveScene)
        {
            lastActiveScene = currentScene;

            // Remove any previous "(not in build index)" label to avoid duplicates
            sceneNames = sceneNames.Where(name => !name.EndsWith(" (not in build index)")).ToArray();

            SelectCurrentScene();
        }
    }


    static void LoadScene(string sceneName)
    {
        string scenePath;

        if (fetchAllScenes)
        {
            scenePath = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
                .FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == sceneName);
        }
        else
        {
            scenePath = EditorBuildSettings.scenes
                .FirstOrDefault(scene => scene.enabled && scene.path.Contains(sceneName))?.path;
        }

        if (!string.IsNullOrEmpty(scenePath))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
        else
        {
            Debug.LogError("Scene not found: " + sceneName);
        }
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
        {
            EditorApplication.delayCall += () => AddToolbarUI();
        }
    }
}
#endif
