using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float fadeDuration = 0.5f;

    [SerializeField]
    private Ease fadeEase = Ease.InOutSine;

    [SerializeField]
    private bool disableInputDuringTransition = true;

    [SerializeField]
    private bool enableDebugLogs;

    private bool isTransitioning;

    public event Action OnShowComplete;
    public event Action OnHideComplete;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public virtual void Show(bool useTransition = true)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        Log("Show called.");
        gameObject.SetActive(true);
        SetInput(false);

        canvasGroup.DOKill();
        if (useTransition)
        {
            canvasGroup.DOFade(1f, fadeDuration)
                .SetEase(fadeEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    SetInput(true);
                    isTransitioning = false;
                    Log("Show complete.");
                    OnShowComplete?.Invoke();
                });
        }
        else
        {
            canvasGroup.alpha = 1f;
            isTransitioning = false;
            OnShowComplete?.Invoke();
        }
    }

    public virtual void Hide(bool useTransition = true)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        Log("Hide called.");
        SetInput(false);

        canvasGroup.DOKill();
        if (useTransition)
        {
            canvasGroup.DOFade(0f, fadeDuration)
                .SetEase(fadeEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    SetInput(true);
                    isTransitioning = false;
                    Log("Hide complete.");
                    OnHideComplete?.Invoke();
                });
        }
        else
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            isTransitioning = false;
            OnHideComplete?.Invoke();
        }
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf && canvasGroup.alpha > 0f;
    }

    private void SetInput(bool state)
    {
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
            Debug.Log($"[BaseUI] {message}");
    }
}