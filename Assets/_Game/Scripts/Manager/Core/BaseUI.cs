using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;

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
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public virtual void Show(bool useTransition = true)
    {
        gameObject.SetActive(true);
        EnableInput();

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
                    gameObject.SetActive(false);
                    EnableInput();
                    OnHideComplete?.Invoke();
                });
        }
        else
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            OnHideComplete?.Invoke();
        }
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf && canvasGroup.alpha > 0f;
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