using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
public abstract class BaseUI : MonoBehaviour
{
    [FoldoutGroup("Canvas Settings"), SerializeField, Required] private Canvas canvas;
    [FoldoutGroup("Canvas Settings"), SerializeField, Required] private CanvasGroup canvasGroup;

    [FoldoutGroup("Transition Settings"), SerializeField]
    private float fadeDuration = 0.5f;

    [FoldoutGroup("Transition Settings"), SerializeField]
    private Ease fadeEase = Ease.InOutSine;

    [FoldoutGroup("Input Settings"), SerializeField]
    private bool disableInputDuringTransition = true;

    public event Action OnShowComplete;
    public event Action OnHideComplete;

    protected virtual void Awake()
    {
        if (canvas == null) canvas = GetComponent<Canvas>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        canvas.enabled = false;
        canvasGroup.alpha = 0f;
    }

    public virtual void Show(bool useTransition = true)
    {
        canvas.enabled = true;

        if (useTransition)
        {
            if (disableInputDuringTransition) DisableInput();
            canvasGroup.DOFade(1f, fadeDuration)
                .SetEase(fadeEase)
                .OnComplete(() =>
                {
                    EnableInput();
                    OnShowComplete?.Invoke();
                });
        }
        else
        {
            canvasGroup.alpha = 1f;
            OnShowComplete?.Invoke();
        }
    }

    public virtual void Hide(bool useTransition = true)
    {
        if (useTransition)
        {
            if (disableInputDuringTransition) DisableInput();
            canvasGroup.DOFade(0f, fadeDuration)
                .SetEase(fadeEase)
                .OnComplete(() =>
                {
                    canvas.enabled = false;
                    EnableInput();
                    OnHideComplete?.Invoke();
                });
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvas.enabled = false;
            OnHideComplete?.Invoke();
        }
    }
    public void SetSortingOrder(int order)
    {
        if (canvas != null)
        {
            canvas.sortingOrder = order;
        }
    }

    public bool IsVisible()
    {
        return canvas.enabled && canvasGroup.alpha > 0f;
    }

    private void DisableInput()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void EnableInput()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
