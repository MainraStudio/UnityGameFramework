using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [FoldoutGroup("UI Prefabs"), SerializeField]
    private List<BaseUI> uiPrefabs;

    private Dictionary<System.Type, BaseUI> uiInstances = new Dictionary<System.Type, BaseUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ShowUI<T>(bool useTransition = true) where T : BaseUI
    {
        var ui = GetOrCreateUIInstance<T>();
        if (ui != null)
        {
            ui.Show(useTransition);
        }

        HideOtherUI<T>(useTransition);
    }
    public void ShowPopupUI(BaseUI popupPrefab, bool useTransition = true, int sortingOrder = 100)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("Popup Prefab is null!");
            return;
        }

        var popupInstance = Instantiate(popupPrefab, transform);
        popupInstance.SetSortingOrder(sortingOrder);
        popupInstance.Show(useTransition);
    }

    public void HideAllUI(bool useTransition = true)
    {
        foreach (var ui in uiInstances.Values)
        {
            ui.Hide(useTransition);
        }
    }

    private T GetOrCreateUIInstance<T>() where T : BaseUI
    {
        var type = typeof(T);

        if (uiInstances.ContainsKey(type))
        {
            return uiInstances[type] as T;
        }

        var prefab = uiPrefabs.Find(p => p is T);
        if (prefab != null)
        {
            var instance = Instantiate(prefab, transform);
            uiInstances[type] = instance;
            return instance as T;
        }

        Debug.LogError($"UI Prefab of type {type.Name} not found!");
        return null;
    }

    private void HideOtherUI<T>(bool useTransition) where T : BaseUI
    {
        foreach (var ui in uiInstances.Values)
        {
            if (!(ui is T))
            {
                ui.Hide(useTransition);
            }
        }
    }
}