#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public class VContainerEditorWindow : EditorWindow
{
    #region Configuration Classes
    [Serializable]
    private class WindowState
    {
        public bool showMainInstructions = true;
        public List<bool> registrationExpansion = new List<bool>();
        public int selectedRegistration = -1;
    }
    #endregion

    #region Core Properties
    private DependencyConfiguration _config;
    private SerializedObject _serializedConfig;
    private SerializedProperty _registrationsProperty;
    private Vector2 _scrollPosition;
    private WindowState _windowState = new WindowState();
    private bool _hasUnsavedChanges;
    #endregion

    #region Initialization
    [MenuItem("Window/VContainer/Dependency Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<VContainerEditorWindow>();
        window.titleContent = new GUIContent("VContainer Manager",
            EditorGUIUtility.IconContent("d_Prefab Icon").image);
        window.minSize = new Vector2(500, 400);
    }

    private void OnEnable()
    {
        LoadConfiguration();
        InitializeState();
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
        SaveChanges();
    }
    #endregion

    #region Core Logic
    private void LoadConfiguration()
    {
        if (_config != null)
        {
            _serializedConfig = new SerializedObject(_config);
            _registrationsProperty = _serializedConfig.FindProperty("Registrations");
            InitializeState();
        }
    }

    private void InitializeState()
    {
        if (_registrationsProperty != null)
        {
            while (_windowState.registrationExpansion.Count < _registrationsProperty.arraySize)
            {
                _windowState.registrationExpansion.Add(false);
            }
        }
    }
    #endregion

    #region GUI Rendering
    private void OnGUI()
    {
        EditorGUILayout.Space(5);
        DrawConfigSelector();

        if (_config == null)
        {
            EditorGUILayout.HelpBox(
                "Please select or create a configuration asset to begin",
                MessageType.Info
            );
            return;
        }

        _serializedConfig.Update();

        using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPosition))
        {
            _scrollPosition = scroll.scrollPosition;
            DrawHeader();
            DrawMainContent();
        }

        DrawSaveControls();
        HandleKeyboardEvents();
    }

    private void DrawConfigSelector()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        DependencyConfiguration newConfig = (DependencyConfiguration)EditorGUILayout.ObjectField(
            "Configuration Asset:",
            _config,
            typeof(DependencyConfiguration),
            false
        );

        if (EditorGUI.EndChangeCheck() && newConfig != _config)
        {
            _config = newConfig;
            LoadConfiguration();
        }

        if (GUILayout.Button("Create New", GUILayout.Width(80)))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Configuration",
                "DependencyConfig",
                "asset",
                "Choose location for new configuration"
            );

            if (!string.IsNullOrEmpty(path))
            {
                var config = ScriptableObject.CreateInstance<DependencyConfiguration>();
                AssetDatabase.CreateAsset(config, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                _config = config;
                LoadConfiguration();
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("VContainer Dependency System", EditorStyles.largeLabel);

        _windowState.showMainInstructions = EditorGUILayout.Foldout(
            _windowState.showMainInstructions,
            "📖 Quick Start Guide",
            true
        );

        if (_windowState.showMainInstructions)
        {
            EditorGUILayout.HelpBox(
                "1. Use '+ Add' to create new registrations\n" +
                "2. Expand items to configure dependencies\n" +
                "3. Save changes when ready\n" +
                "4. Assign config to DependencyInstaller component",
                MessageType.Info);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawMainContent()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Dependency Registrations", EditorStyles.boldLabel);

        if (_registrationsProperty == null) return;

        for (int i = 0; i < _registrationsProperty.arraySize; i++)
        {
            DrawRegistrationItem(i);
        }

        if (GUILayout.Button("+ Add New Registration"))
        {
            AddNewRegistration();
        }
    }

    private void DrawRegistrationItem(int index)
    {
        var element = _registrationsProperty.GetArrayElementAtIndex(index);
        var interfaceType = element.FindPropertyRelative("InterfaceType");
        var implementationType = element.FindPropertyRelative("ImplementationType");
        var lifetime = element.FindPropertyRelative("Lifetime");

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _windowState.registrationExpansion[index] = EditorGUILayout.Foldout(
            _windowState.registrationExpansion[index],
            $"📦 Registration {index + 1} - {GetTypeName(implementationType)}",
            true
        );

        if (_windowState.registrationExpansion[index])
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(
                interfaceType,
                new GUIContent("Contract Type", "Interface/Abstract class to register against")
            );

            EditorGUILayout.PropertyField(
                implementationType,
                new GUIContent("Concrete Type", "Actual implementation class")
            );

            EditorGUILayout.PropertyField(
                lifetime,
                new GUIContent("Lifetime Scope", "Instance management strategy")
            );

            if (EditorGUI.EndChangeCheck())
            {
                _serializedConfig.ApplyModifiedProperties();
                _hasUnsavedChanges = true;
            }

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = index > 0;
            if (GUILayout.Button("▲ Up"))
            {
                MoveRegistration(index, -1);
            }
            GUI.enabled = index < _registrationsProperty.arraySize - 1;
            if (GUILayout.Button("▼ Down"))
            {
                MoveRegistration(index, 1);
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                RemoveRegistration(index);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    private void DrawSaveControls()
    {
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.enabled = _hasUnsavedChanges;
        if (GUILayout.Button(_hasUnsavedChanges ? "💾 Save Changes*" : "✅ All Changes Saved",
            GUILayout.Width(200), GUILayout.Height(30)))
        {
            SaveChanges();
        }
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Functionality
    private void AddNewRegistration()
    {
        if (_config == null) return;

        Undo.RecordObject(_config, "Add Registration");
        _registrationsProperty.arraySize++;
        _windowState.registrationExpansion.Add(true);
        _hasUnsavedChanges = true;
        _serializedConfig.ApplyModifiedProperties();
    }

    private void MoveRegistration(int index, int direction)
    {
        int newIndex = Mathf.Clamp(index + direction, 0, _registrationsProperty.arraySize - 1);
        if (newIndex != index)
        {
            Undo.RecordObject(_config, "Move Registration");
            _registrationsProperty.MoveArrayElement(index, newIndex);
            var expansionState = _windowState.registrationExpansion[index];
            _windowState.registrationExpansion[index] = _windowState.registrationExpansion[newIndex];
            _windowState.registrationExpansion[newIndex] = expansionState;
            _hasUnsavedChanges = true;
            _serializedConfig.ApplyModifiedProperties();
        }
    }

    private void RemoveRegistration(int index)
    {
        if (EditorUtility.DisplayDialog("Confirm Removal",
            "Delete this registration?", "Yes", "No"))
        {
            Undo.RecordObject(_config, "Remove Registration");
            _registrationsProperty.DeleteArrayElementAtIndex(index);
            _windowState.registrationExpansion.RemoveAt(index);
            _hasUnsavedChanges = true;
            _serializedConfig.ApplyModifiedProperties();
        }
    }

    private void SaveChanges()
    {
        if (!_hasUnsavedChanges) return;

        try
        {
            _serializedConfig.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(_config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _hasUnsavedChanges = false;
            Debug.Log("[VContainer] Configuration saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"[VContainer] Save failed: {e.Message}");
        }
    }

    private string GetTypeName(SerializedProperty property)
    {
        var script = property.objectReferenceValue as MonoScript;
        return script != null ? script.GetClass().Name : "Not Set";
    }
    #endregion

    #region Event Handlers
    private void HandleKeyboardEvents()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.control && Event.current.keyCode == KeyCode.S)
            {
                SaveChanges();
                Event.current.Use();
            }
        }
    }

    private void OnUndoRedo()
    {
        LoadConfiguration();
        Repaint();
    }
    #endregion
}
#endif