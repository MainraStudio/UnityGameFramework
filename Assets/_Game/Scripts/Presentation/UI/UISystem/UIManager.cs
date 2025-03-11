using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Presentation.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace _Game.Scripts.Application.Manager.Core.UISystem
{
    public class UIManager : PersistentSingleton<UIManager>
    {
        public static event Action OnButtonClicked;

        [TabGroup("Prefabs"), ListDrawerSettings(Expanded = true)]
        [SerializeField] private List<BaseUI> persistentUIPrefabs;

        [TabGroup("Prefabs"), ListDrawerSettings(Expanded = true)]
        [SerializeField] private List<BaseUI> popupUIPrefabs;

        [TabGroup("Prefabs"), ListDrawerSettings(Expanded = true)]
        [SerializeField] private List<BaseUI> worldUIPrefabs;

        [TabGroup("Canvases")]
        [SerializeField] private Canvas persistentCanvas;
        [TabGroup("Canvases")]
        [SerializeField] private Canvas popupCanvas;
        [TabGroup("Canvases")]
        [SerializeField] private Canvas worldSpaceCanvas;

        private Dictionary<System.Type, BaseUI> uiInstances = new Dictionary<System.Type, BaseUI>();
        private HashSet<System.Type> persistentUI = new HashSet<System.Type>();
        private readonly Dictionary<System.Type, List<BaseUI>> worldSpaceInstances = 
            new Dictionary<System.Type, List<BaseUI>>();

        private T GetPrefabByType<T>(List<BaseUI> prefabList) where T : BaseUI
        {
            var prefab = prefabList.Find(p => p is T);
            if (prefab == null)
            {
                Debug.LogError($"UI Prefab of type {typeof(T).Name} not found!");
                return null;
            }
            return prefab as T;
        }

        private T GetOrCreateUIInstance<T>(Transform parent, List<BaseUI> prefabList) where T : BaseUI
        {
            var type = typeof(T);

            if (uiInstances.ContainsKey(type))
            {
                return uiInstances[type] as T;
            }

            var prefab = GetPrefabByType<T>(prefabList);
            if (prefab != null)
            {
                var instance = Instantiate(prefab, parent);
                uiInstances[type] = instance;
                return instance as T;
            }

            return null;
        }

        public void ShowUI<T>(bool useTransition = true) where T : BaseUI
        {
            EnableCanvas(persistentCanvas);
            var ui = GetOrCreateUIInstance<T>(persistentCanvas.transform, persistentUIPrefabs);
            if (ui != null)
            {
                ui.Show(useTransition);
            }

            HideOtherUI<T>(useTransition);
        }

        public void ShowPopupUI<T>(bool useTransition = true) where T : BaseUI
        {
            EnableCanvas(popupCanvas);
            var popup = GetOrCreateUIInstance<T>(popupCanvas.transform, popupUIPrefabs);
            if (popup != null)
            {
                popup.Show(useTransition);
            }
        }

        public T ShowUIAtWorldPosition<T>(Vector3 worldPosition, bool useTransition = true) where T : BaseUI
        {
            EnableCanvas(worldSpaceCanvas);
            var prefab = GetPrefabByType<T>(worldUIPrefabs);
            if (prefab == null) return null;

            var instance = Instantiate(prefab, worldSpaceCanvas.transform);
            instance.transform.position = worldPosition;

            var type = typeof(T);
            if (!worldSpaceInstances.ContainsKey(type))
            {
                worldSpaceInstances[type] = new List<BaseUI>();
            }
            worldSpaceInstances[type].Add(instance);

            instance.Show(useTransition);
            return instance;
        }

        public void ShowConfirmationPopup(string message, UnityAction onConfirm, UnityAction onCancel)
        {
            var popup = GetOrCreateUIInstance<ConfirmationPopup>(popupCanvas.transform, popupUIPrefabs);
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

            foreach (var instances in worldSpaceInstances.Values)
            {
                foreach (var ui in instances.ToList())
                {
                    if (ui != null)
                    {
                        ui.Hide(useTransition);
                        Destroy(ui.gameObject);
                    }
                }
                instances.Clear();
            }

            DisableCanvas(persistentCanvas);
            DisableCanvas(popupCanvas);
            DisableCanvas(worldSpaceCanvas);
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

        public void HideAllWorldSpaceUI<T>(bool useTransition = true) where T : BaseUI
        {
            var type = typeof(T);
            if (worldSpaceInstances.TryGetValue(type, out var instances))
            {
                foreach (var ui in instances.ToList())
                {
                    if (ui != null)
                    {
                        ui.Hide(useTransition);
                        Destroy(ui.gameObject);
                    }
                }
                worldSpaceInstances[type].Clear();
            }

            DisableCanvasIfNoActiveUI(worldSpaceCanvas);
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
}