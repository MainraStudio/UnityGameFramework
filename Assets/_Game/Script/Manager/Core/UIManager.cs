using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private List<BaseUI> uiElements;

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

    public void ShowUI<T>() where T : BaseUI
    {
        foreach (var ui in uiElements)
        {
            if (ui is T)
            {
                ui.Show();
            }
            else
            {
                ui.Hide();
            }
        }
    }

    public void HideAllUI()
    {
        foreach (var ui in uiElements)
        {
            ui.Hide();
        }
    }

    public void RegisterUI(BaseUI ui)
    {
        if (!uiElements.Contains(ui))
        {
            uiElements.Add(ui);
        }
    }

    public void UnregisterUI(BaseUI ui)
    {
        if (uiElements.Contains(ui))
        {
            uiElements.Remove(ui);
        }
    }
}