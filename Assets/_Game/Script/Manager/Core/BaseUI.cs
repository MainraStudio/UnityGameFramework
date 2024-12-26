using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    protected virtual void Awake()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
    }

    public virtual void Show()
    {
        if (canvas != null)
        {
            canvas.enabled = true;
        }
    }

    public virtual void Hide()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    public bool IsVisible()
    {
        return canvas != null && canvas.enabled;
    }
}