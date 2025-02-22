#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using VContainer.Unity;

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
	private string _searchFilter = "";
    #endregion

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
	private void LoadConfiguration()
    {
        if (_config == null)
        {
            _serializedConfig = null;
            _registrationsProperty = null;
            _windowState.registrationExpansion.Clear();
            return;
        }

        _serializedConfig = new SerializedObject(_config);
        _registrationsProperty = _serializedConfig.FindProperty("Registrations");
        InitializeState();
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

        DrawSearchBar();

        using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPosition))
        {
            _scrollPosition = scroll.scrollPosition;
            DrawHeader();
            DrawMainContent();
        }

        DrawSaveControls();
        HandleKeyboardEvents();
        HandleDragAndDrop();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        string newFilter = EditorGUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
        
        if (newFilter != _searchFilter)
        {
            _searchFilter = newFilter;
            // Reset expansion states when filter changes
            if (!string.IsNullOrEmpty(_searchFilter))
            {
                for (int i = 0; i < _windowState.registrationExpansion.Count; i++)
                {
                    _windowState.registrationExpansion[i] = true;
                }
            }
        }

        if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            _searchFilter = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();
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
            if (_hasUnsavedChanges)
            {
                if (EditorUtility.DisplayDialog("Unsaved Changes",
                    "There are unsaved changes. Do you want to save them before switching?",
                    "Save", "Discard"))
                {
                    SaveChanges();
                }
            }
            _config = newConfig;
            LoadConfiguration();
        }

        if (GUILayout.Button("Create New", GUILayout.Width(80)))
        {
            CreateNewConfiguration();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void CreateNewConfiguration()
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
                "2. Configure the registration type and properties\n" +
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
            if (ShouldShowRegistration(i))
            {
                DrawRegistrationItem(i);
            }
        }

        if (GUILayout.Button("+ Add New Registration"))
        {
            AddNewRegistration();
        }
    }

    private bool ShouldShowRegistration(int index)
    {
        if (string.IsNullOrEmpty(_searchFilter)) return true;

        var element = _registrationsProperty.GetArrayElementAtIndex(index);
        var implType = element.FindPropertyRelative("ImplementationType");
        var interfaceType = element.FindPropertyRelative("InterfaceType");
        var factoryType = element.FindPropertyRelative("FactoryType");

        var typeName = GetTypeName(implType) ?? "";
        var interfaceName = GetTypeName(interfaceType) ?? "";
        var factoryName = GetTypeName(factoryType) ?? "";

        return typeName.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
               interfaceName.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
               factoryName.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void HandleDragAndDrop()
    {
        if (Event.current.type == EventType.DragUpdated)
        {
            bool validDrag = DragAndDrop.objectReferences.Length > 0 &&
                            (DragAndDrop.objectReferences[0] is MonoScript || 
                             DragAndDrop.objectReferences[0] is GameObject);

            DragAndDrop.visualMode = validDrag ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
            Event.current.Use();
        }
        else if (Event.current.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            foreach (var obj in DragAndDrop.objectReferences)
            {
                AddNewRegistrationFromObject(obj);
            }
            Event.current.Use();
        }
    }

    private void AddNewRegistrationFromObject(UnityEngine.Object obj)
    {
        if (_config == null) return;

        Undo.RecordObject(_config, "Add Registration From Object");
        _registrationsProperty.arraySize++;
        var newElement = _registrationsProperty.GetArrayElementAtIndex(_registrationsProperty.arraySize - 1);

        if (obj is MonoScript script)
        {
            var type = script.GetClass();
            if (type != null)
            {
                if (type.IsInterface)
                {
                    newElement.FindPropertyRelative("RegistrationType").enumValueIndex = 
                        (int)RegistrationType.Interface;
                    newElement.FindPropertyRelative("InterfaceType").objectReferenceValue = script;
                }
                else if (typeof(Component).IsAssignableFrom(type))
                {
                    newElement.FindPropertyRelative("RegistrationType").enumValueIndex = 
                        (int)RegistrationType.ComponentInScene;
                    newElement.FindPropertyRelative("ImplementationType").objectReferenceValue = script;
                }
                else if (typeof(IFactory).IsAssignableFrom(type))
                {
                    newElement.FindPropertyRelative("RegistrationType").enumValueIndex = 
                        (int)RegistrationType.Factory;
                    newElement.FindPropertyRelative("FactoryType").objectReferenceValue = script;
                }
                else
                {
                    newElement.FindPropertyRelative("RegistrationType").enumValueIndex = 
                        (int)RegistrationType.Class;
                    newElement.FindPropertyRelative("ImplementationType").objectReferenceValue = script;
                }
            }
        }
        else if (obj is GameObject go)
        {
            newElement.FindPropertyRelative("RegistrationType").enumValueIndex = 
                (int)RegistrationType.ComponentInPrefab;
            newElement.FindPropertyRelative("Prefab").objectReferenceValue = go;
        }

        _windowState.registrationExpansion.Add(true);
        _hasUnsavedChanges = true;
        _serializedConfig.ApplyModifiedProperties();
    }
    private void DrawRegistrationItem(int index)
    {
        var element = _registrationsProperty.GetArrayElementAtIndex(index);
        var registrationType = element.FindPropertyRelative("RegistrationType");
        var interfaceType = element.FindPropertyRelative("InterfaceType");
        var implementationType = element.FindPropertyRelative("ImplementationType");
        var factoryType = element.FindPropertyRelative("FactoryType");
        var sceneObject = element.FindPropertyRelative("SceneObject");
        var instance = element.FindPropertyRelative("Instance");
        var prefab = element.FindPropertyRelative("Prefab");
        var lifetime = element.FindPropertyRelative("registrationLifetime");

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _windowState.registrationExpansion[index] = EditorGUILayout.Foldout(
            _windowState.registrationExpansion[index],
            $"📦 Registration {index + 1} - {GetRegistrationTitle(element)}",
            true
        );

        if (_windowState.registrationExpansion[index])
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(registrationType);

            var type = (RegistrationType)registrationType.enumValueIndex;
            switch (type)
            {
                case RegistrationType.Interface:
                    EditorGUILayout.PropertyField(interfaceType);
                    EditorGUILayout.PropertyField(implementationType);
                    ValidateInterfaceRegistration(interfaceType, implementationType);
                    break;

                case RegistrationType.Class:
                    EditorGUILayout.PropertyField(implementationType);
                    ValidateClassRegistration(implementationType);
                    break;

                case RegistrationType.ComponentInScene:
                    EditorGUILayout.PropertyField(implementationType);
                    EditorGUILayout.PropertyField(sceneObject);
                    ValidateComponentRegistration(implementationType, sceneObject);
                    break;

                case RegistrationType.Instance:
                    EditorGUILayout.PropertyField(instance);
                    ValidateInstanceRegistration(instance);
                    break;

                case RegistrationType.ComponentInPrefab:
                    EditorGUILayout.PropertyField(implementationType);
                    EditorGUILayout.PropertyField(prefab);
                    ValidateComponentInPrefabRegistration(implementationType, prefab);
                    break;

                case RegistrationType.Factory:
                    EditorGUILayout.PropertyField(factoryType);
                    ValidateFactoryRegistration(factoryType);
                    break;

                case RegistrationType.EntryPoint:
                    EditorGUILayout.PropertyField(implementationType);
                    ValidateEntryPointRegistration(implementationType);
                    break;
            }

            EditorGUILayout.PropertyField(lifetime);

            if (EditorGUI.EndChangeCheck())
            {
                _serializedConfig.ApplyModifiedProperties();
                _hasUnsavedChanges = true;
            }

            DrawRegistrationControls(index);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    private void DrawRegistrationControls(int index)
    {
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
    }

    private string GetRegistrationTitle(SerializedProperty element)
    {
        var type = (RegistrationType)element.FindPropertyRelative("RegistrationType").enumValueIndex;
        switch (type)
        {
            case RegistrationType.Interface:
                return $"Interface: {GetTypeName(element.FindPropertyRelative("InterfaceType"))}";
            case RegistrationType.Factory:
                return $"Factory: {GetTypeName(element.FindPropertyRelative("FactoryType"))}";
            default:
                return $"{type}: {GetTypeName(element.FindPropertyRelative("ImplementationType"))}";
        }
    }
    private void ValidateInterfaceRegistration(SerializedProperty interfaceType, SerializedProperty implType)
    {
        var interfaceScript = interfaceType.objectReferenceValue as MonoScript;
        var implScript = implType.objectReferenceValue as MonoScript;

        if (interfaceScript == null)
        {
            EditorGUILayout.HelpBox("Interface type is required", MessageType.Error);
            return;
        }

        var interfaceClass = interfaceScript.GetClass();
        if (interfaceClass == null || !interfaceClass.IsInterface)
        {
            EditorGUILayout.HelpBox("Selected type must be an interface", MessageType.Error);
            return;
        }

        if (implScript != null)
        {
            var implClass = implScript.GetClass();
            if (implClass != null && !interfaceClass.IsAssignableFrom(implClass))
            {
                EditorGUILayout.HelpBox(
                    $"Implementation must implement {interfaceClass.Name}",
                    MessageType.Error
                );
            }
        }
    }

    private void ValidateClassRegistration(SerializedProperty implType)
    {
        var script = implType.objectReferenceValue as MonoScript;
        if (script == null)
        {
            EditorGUILayout.HelpBox("Implementation type is required", MessageType.Error);
            return;
        }

        var type = script.GetClass();
        if (type == null || type.IsInterface || type.IsAbstract)
        {
            EditorGUILayout.HelpBox("Type must be a concrete class", MessageType.Error);
        }
    }

    private void ValidateComponentRegistration(SerializedProperty implType, SerializedProperty sceneObject)
    {
        var script = implType.objectReferenceValue as MonoScript;
        if (script == null)
        {
            EditorGUILayout.HelpBox("Component type is required", MessageType.Error);
            return;
        }

        var type = script.GetClass();
        if (type == null || !typeof(Component).IsAssignableFrom(type))
        {
            EditorGUILayout.HelpBox("Type must inherit from Component", MessageType.Error);
            return;
        }

        if (sceneObject.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Scene object reference is required", MessageType.Warning);
            return;
        }

        var go = sceneObject.objectReferenceValue as GameObject;
        if (go != null && go.GetComponent(type) == null)
        {
            EditorGUILayout.HelpBox(
                $"Selected object does not have a {type.Name} component",
                MessageType.Error
            );
        }
    }

    private void ValidateInstanceRegistration(SerializedProperty instance)
    {
        if (instance.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Instance reference is required", MessageType.Error);
        }
    }

    private void ValidateComponentInPrefabRegistration(SerializedProperty implType, SerializedProperty prefab)
    {
        var script = implType.objectReferenceValue as MonoScript;
        if (script == null)
        {
            EditorGUILayout.HelpBox("Component type is required", MessageType.Error);
            return;
        }

        var type = script.GetClass();
        if (type == null || !typeof(Component).IsAssignableFrom(type))
        {
            EditorGUILayout.HelpBox("Type must inherit from Component", MessageType.Error);
            return;
        }

        if (prefab.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Prefab reference is required", MessageType.Warning);
            return;
        }

        var prefabGo = prefab.objectReferenceValue as GameObject;
        if (prefabGo != null && prefabGo.GetComponent(type) == null)
        {
            EditorGUILayout.HelpBox(
                $"Selected prefab does not have a {type.Name} component",
                MessageType.Error
            );
        }
    }
    private void ValidateFactoryRegistration(SerializedProperty factoryType)
    {
        var script = factoryType.objectReferenceValue as MonoScript;
        if (script == null)
        {
            EditorGUILayout.HelpBox("Factory type is required", MessageType.Error);
            return;
        }

        var type = script.GetClass();
        if (type == null || !typeof(IFactory).IsAssignableFrom(type))
        {
            EditorGUILayout.HelpBox("Type must implement IFactory", MessageType.Error);
        }
    }

    private void ValidateEntryPointRegistration(SerializedProperty implType)
    {
        var script = implType.objectReferenceValue as MonoScript;
        if (script == null)
        {
            EditorGUILayout.HelpBox("Implementation type is required", MessageType.Error);
            return;
        }

        var type = script.GetClass();
        if (type == null || !typeof(IStartable).IsAssignableFrom(type))
        {
            EditorGUILayout.HelpBox("Type must implement IStartable", MessageType.Error);
        }
    }

    private string GetTypeName(SerializedProperty property)
    {
        var script = property.objectReferenceValue as MonoScript;
        if (script == null) return null;
        var type = script.GetClass();
        return type?.Name;
    }

    private void AddNewRegistration()
    {
        Undo.RecordObject(_config, "Add Registration");
        _registrationsProperty.arraySize++;
        _windowState.registrationExpansion.Add(true);
        _hasUnsavedChanges = true;
        _serializedConfig.ApplyModifiedProperties();
    }

    private void RemoveRegistration(int index)
    {
        if (EditorUtility.DisplayDialog("Remove Registration",
            "Are you sure you want to remove this registration?",
            "Remove", "Cancel"))
        {
            Undo.RecordObject(_config, "Remove Registration");
            _registrationsProperty.DeleteArrayElementAtIndex(index);
            _windowState.registrationExpansion.RemoveAt(index);
            _hasUnsavedChanges = true;
            _serializedConfig.ApplyModifiedProperties();
        }
    }

    private void MoveRegistration(int index, int offset)
    {
        _registrationsProperty.MoveArrayElement(index, index + offset);
        var tempExpansion = _windowState.registrationExpansion[index];
        _windowState.registrationExpansion[index] = _windowState.registrationExpansion[index + offset];
        _windowState.registrationExpansion[index + offset] = tempExpansion;
        _hasUnsavedChanges = true;
        _serializedConfig.ApplyModifiedProperties();
    }

    private void HandleKeyboardEvents()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            if (e.control && e.keyCode == KeyCode.S)
            {
                SaveChanges();
                e.Use();
            }
        }
    }

    private void DrawSaveControls()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.enabled = _hasUnsavedChanges;
        if (GUILayout.Button("Save Changes", GUILayout.Width(120)))
        {
            SaveChanges();
        }
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }

    private void SaveChanges()
    {
        if (_config != null && _hasUnsavedChanges)
        {
            EditorUtility.SetDirty(_config);
            AssetDatabase.SaveAssets();
            _hasUnsavedChanges = false;
        }
    }

    private void OnUndoRedo()
    {
        if (_config != null)
        {
            _serializedConfig.Update();
            InitializeState();
            Repaint();
        }
    }
}
#endif