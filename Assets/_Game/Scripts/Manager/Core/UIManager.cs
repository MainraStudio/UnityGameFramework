using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using NaughtyAttributes;

public class UIManager : PersistentSingleton<UIManager>
{
    public static event Action OnButtonClicked;

    [ReorderableList]
    [SerializeField] private List<BaseUI> uiPrefabs;
    
    [ReorderableList]
    [SerializeField] private List<BaseUI> popupPrefabs;

    [SerializeField] private Canvas persistentCanvas;
    [SerializeField] private Canvas popupCanvas;

    private Dictionary<System.Type, BaseUI> uiInstances = new Dictionary<System.Type, BaseUI>();
    private HashSet<System.Type> persistentUI = new HashSet<System.Type>();

    public void ShowUI<T>(bool useTransition = true) where T : BaseUI
    {
        EnableCanvas(persistentCanvas);
        var ui = GetOrCreateUIInstance<T>(persistentCanvas.transform);
        if (ui != null)
        {
            ui.Show(useTransition);
        }

        HideOtherUI<T>(useTransition);
    }

    public void ShowPopupUI<T>(bool useTransition = true) where T : BaseUI
    {
        EnableCanvas(popupCanvas);
        var popup = GetOrCreatePopupInstance<T>(popupCanvas.transform);
        if (popup != null)
        {
            popup.Show(useTransition);
        }
    }

    public void ShowConfirmationPopup(string message, UnityAction onConfirm, UnityAction onCancel)
    {
        var popup = GetOrCreatePopupInstance<ConfirmationPopup>(popupCanvas.transform);
        if (popup != null)
        {
            popup.Setup(message, onConfirm, onCancel);
            popup.Show(true);
        }
    }

    public void HideAllUI(bool useTransition = true)
    {
        foreach (var ui in uiInstances.Values)
        {
            if (!persistentUI.Contains(ui.GetType()))
            {
                ui.Hide(useTransition);
            }
        }

        DisableCanvas(persistentCanvas);
        DisableCanvas(popupCanvas);
    }

    public void HideUI<T>(bool useTransition = true) where T : BaseUI
    {
        var ui = GetUIInstance<T>();
        if (ui != null)
        {
            ui.Hide(useTransition);
        }

        DisableCanvasIfNoActiveUI(persistentCanvas);
        DisableCanvasIfNoActiveUI(popupCanvas);
    }

    private void EnableCanvas(Canvas canvas)
    {
        if (canvas != null)
        {
            canvas.enabled = true;
        }
    }

    private void DisableCanvas(Canvas canvas)
    {
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    private void DisableCanvasIfNoActiveUI(Canvas canvas)
    {
        bool hasActiveUI = false;
        foreach (var ui in uiInstances.Values)
        {
            if (ui.transform.parent == canvas.transform && ui.IsVisible())
            {
                hasActiveUI = true;
                break;
            }
        }

        if (!hasActiveUI)
        {
            DisableCanvas(canvas);
        }
    }

    public void ToggleUI<T>(bool useTransition = true) where T : BaseUI
    {
        var ui = GetUIInstance<T>();
        if (ui != null)
        {
            if (ui.IsVisible())
            {
                ui.Hide(useTransition);
            }
            else
            {
                EnableCanvas(persistentCanvas);
                ui.Show(useTransition);
            }
        }

        DisableCanvasIfNoActiveUI(persistentCanvas);
    }

    public bool IsUIVisible<T>() where T : BaseUI
    {
        var ui = GetUIInstance<T>();
        return ui != null && ui.IsVisible();
    }

    public T GetUIInstance<T>() where T : BaseUI
    {
        var type = typeof(T);
        if (uiInstances.ContainsKey(type))
        {
            return uiInstances[type] as T;
        }

        return null;
    }

    private T GetOrCreateUIInstance<T>(Transform parent) where T : BaseUI
    {
        var type = typeof(T);

        if (uiInstances.ContainsKey(type))
        {
            return uiInstances[type] as T;
        }

        var prefab = uiPrefabs.Find(p => p is T);
        if (prefab != null)
        {
            var instance = Instantiate(prefab, parent);
            uiInstances[type] = instance;
            return instance as T;
        }

        Debug.LogError($"UI Prefab of type {type.Name} not found!");
        return null;
    }

    private T GetOrCreatePopupInstance<T>(Transform parent) where T : BaseUI
    {
        var type = typeof(T);

        if (uiInstances.ContainsKey(type))
        {
            return uiInstances[type] as T;
        }

        var prefab = popupPrefabs.Find(p => p is T);
        if (prefab != null)
        {
            var instance = Instantiate(prefab, parent);
            uiInstances[type] = instance;
            return instance as T;
        }

        Debug.LogError($"Popup Prefab of type {type.Name} not found!");
        return null;
    }

    private void HideOtherUI<T>(bool useTransition) where T : BaseUI
    {
        foreach (var ui in uiInstances.Values)
        {
            if (!(ui is T) && !persistentUI.Contains(ui.GetType()))
            {
                ui.Hide(useTransition);
            }
        }
    }

    public void AddButtonListenerWithSFX(Button button, UnityAction action)
    {
        button.onClick.AddListener(() =>
        {
            OnButtonClicked?.Invoke();
            action.Invoke();
        });
    }

    public void SetUIPersistent<T>() where T : BaseUI
    {
        var type = typeof(T);
        if (!persistentUI.Contains(type))
        {
            persistentUI.Add(type);
        }
    }

    public void RemoveUIPersistent<T>() where T : BaseUI
    {
        var type = typeof(T);
        if (persistentUI.Contains(type))
        {
            persistentUI.Remove(type);
        }
    }
}