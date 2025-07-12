using UnityEngine;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Sequence currentSequence;
    private Vector2 originalPosition;
    private Vector3 originalScale;

    [TabGroup("Transition", "General")]
    [SerializeField] private bool useTransition = true;

    [TabGroup("Transition", "Fade")]
    [EnableIf("useTransition")]
    [SerializeField] private float fadeDuration = 0.5f;
    
    [TabGroup("Transition", "Fade")]
    [EnableIf("useTransition")]
    [SerializeField] private bool useCustomFadeCurve = false;
    
    [TabGroup("Transition", "Fade")]
    [ShowIf("@useTransition && useCustomFadeCurve")]
    [SerializeField] private AnimationCurve customFadeCurve;
    
    [TabGroup("Transition", "Fade")]
    [ShowIf("@useTransition && !useCustomFadeCurve")]
    [SerializeField] private Ease fadeEase = Ease.InOutSine;

    [TabGroup("Transition", "Scale")]
    [EnableIf("useTransition")]
    [SerializeField] private bool useScaleEffect = false;
    
    [TabGroup("Transition", "Scale")]
    [ShowIf("@useTransition && useScaleEffect")]
    [SerializeField] private Vector3 startScale = new Vector3(0.8f, 0.8f, 0.8f);
    
    [TabGroup("Transition", "Scale")]
    [ShowIf("@useTransition && useScaleEffect")]
    [SerializeField] private bool useCustomScaleCurve = false;
    
    [TabGroup("Transition", "Scale")]
    [ShowIf("@useTransition && useScaleEffect && useCustomScaleCurve")]
    [SerializeField] private AnimationCurve customScaleCurve;
    
    [TabGroup("Transition", "Scale")]
    [ShowIf("@useTransition && useScaleEffect && !useCustomScaleCurve")]
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    [TabGroup("Transition", "Position")]
    [EnableIf("useTransition")]
    [SerializeField] private bool usePositionEffect = false;
    
    [TabGroup("Transition", "Position")]
    [ShowIf("@useTransition && usePositionEffect")]
    [SerializeField] private UISlideDirection slideDirection = UISlideDirection.Bottom;
    
    [TabGroup("Transition", "Position")]
    [ShowIf("@useTransition && usePositionEffect")]
    [SerializeField] private float slideDistance = 100f;
    
    [TabGroup("Transition", "Position")]
    [ShowIf("@useTransition && usePositionEffect")]
    [SerializeField] private bool useCustomPositionCurve = false;
    
    [TabGroup("Transition", "Position")]
    [ShowIf("@useTransition && usePositionEffect && useCustomPositionCurve")]
    [SerializeField] private AnimationCurve customPositionCurve;
    
    [TabGroup("Transition", "Position")]
    [ShowIf("@useTransition && usePositionEffect && !useCustomPositionCurve")]
    [SerializeField] private Ease positionEase = Ease.OutQuad;

    [TabGroup("Advanced", "Input")]
    [SerializeField] private bool disableInputDuringTransition = true;
    
    [TabGroup("Advanced", "Input")]
    [SerializeField] private bool ignoreTimeScale = false;

    [TabGroup("Advanced", "Queue")]
    [SerializeField] private bool queueTransitions = false;
    
    [TabGroup("Advanced", "Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    private bool isTransitioning;
    private Queue<TransitionRequest> transitionQueue = new Queue<TransitionRequest>();

    public event Action OnShowStarted;
    public event Action OnShowComplete;
    public event Action OnHideStarted;
    public event Action OnHideComplete;
    public event Action<float> OnTransitionProgress;

    private struct TransitionRequest
    {
        public bool isShow;
        public bool useTransition;
        public Action callback;
    }

    public enum UISlideDirection
    {
        Left, Right, Top, Bottom
    }

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            Debug.LogError($"[{gameObject.name}] CanvasGroup component is missing!");
            return;
        }

        if (rectTransform == null)
        {
            Debug.LogError($"[{gameObject.name}] RectTransform component is missing!");
            return;
        }

        // Store original transform values
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;

        // Initial setup based on useTransition
        if (useTransition)
        {
            // Start hidden with no input
            canvasGroup.alpha = 0f;
            SetInput(true);
            // Show with transition
            Show(true);
        }
        else
        {
            // Start visible with input enabled
            canvasGroup.alpha = 1f;
            SetInput(true);
        }
    }

    protected virtual void OnValidate()
    {
        if (fadeDuration < 0) fadeDuration = 0;
        if (slideDistance < 0) slideDistance = 0;

        // Validate curves
        if (useCustomFadeCurve && customFadeCurve == null)
            Debug.LogWarning($"[{gameObject.name}] Custom fade curve is enabled but no curve is assigned!");
        if (useScaleEffect && useCustomScaleCurve && customScaleCurve == null)
            Debug.LogWarning($"[{gameObject.name}] Custom scale curve is enabled but no curve is assigned!");
        if (usePositionEffect && useCustomPositionCurve && customPositionCurve == null)
            Debug.LogWarning($"[{gameObject.name}] Custom position curve is enabled but no curve is assigned!");
    }

    public virtual void Show(bool useTransition = true, Action callback = null)
    {
        if (isTransitioning && queueTransitions)
        {
            transitionQueue.Enqueue(new TransitionRequest { isShow = true, useTransition = useTransition, callback = callback });
            Log("Show request queued.");
            return;
        }
        else if (isTransitioning)
        {
            Log("Show request ignored - transition in progress.");
            return;
        }

        StartShow(useTransition, callback);
    }

    private void StartShow(bool useTransition = true, Action callback = null)
    {
        isTransitioning = true;
        OnShowStarted?.Invoke();
        Log("Show started.");

        gameObject.SetActive(true);
        if (disableInputDuringTransition)
        {
            SetInput(false);
        }

        KillCurrentTransition();

        if (useTransition)
        {
            currentSequence = DOTween.Sequence().SetUpdate(ignoreTimeScale);

            // Setup initial state
            if (usePositionEffect)
            {
                rectTransform.anchoredPosition = GetOffscreenPosition();
            }
            if (useScaleEffect)
            {
                rectTransform.localScale = startScale;
            }

            // Add fade tween
            var fadeTween = canvasGroup.DOFade(1f, fadeDuration);
            if (useCustomFadeCurve)
                fadeTween.SetEase(customFadeCurve);
            else
                fadeTween.SetEase(fadeEase);
            currentSequence.Join(fadeTween);

            // Add scale tween
            if (useScaleEffect)
            {
                var scaleTween = rectTransform.DOScale(originalScale, fadeDuration);
                if (useCustomScaleCurve)
                    scaleTween.SetEase(customScaleCurve);
                else
                    scaleTween.SetEase(scaleEase);
                currentSequence.Join(scaleTween);
            }

            // Add position tween
            if (usePositionEffect)
            {
                var positionTween = rectTransform.DOAnchorPos(originalPosition, fadeDuration);
                if (useCustomPositionCurve)
                    positionTween.SetEase(customPositionCurve);
                else
                    positionTween.SetEase(positionEase);
                currentSequence.Join(positionTween);
            }

            // Setup progress callback
            currentSequence.OnUpdate(() => {
                OnTransitionProgress?.Invoke(currentSequence.ElapsedPercentage());
            });

            // Setup completion callback
            currentSequence.OnComplete(() => CompleteShow(callback));
        }
        else
        {
            canvasGroup.alpha = 1f;
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.localScale = originalScale;
            CompleteShow(callback);
        }
    }

    private void CompleteShow(Action callback = null)
    {
        if (disableInputDuringTransition)
        {
            SetInput(true);
        }
        isTransitioning = false;
        Log("Show complete.");
        OnShowComplete?.Invoke();
        callback?.Invoke();

        ProcessNextTransition();
    }

    public virtual void Hide(bool useTransition = true, Action callback = null)
    {
        if (isTransitioning && queueTransitions)
        {
            transitionQueue.Enqueue(new TransitionRequest { isShow = false, useTransition = useTransition, callback = callback });
            Log("Hide request queued.");
            return;
        }
        else if (isTransitioning)
        {
            Log("Hide request ignored - transition in progress.");
            return;
        }

        StartHide(useTransition, callback);
    }

    private void StartHide(bool useTransition = true, Action callback = null)
    {
        isTransitioning = true;
        OnHideStarted?.Invoke();
        Log("Hide started.");

        if (disableInputDuringTransition)
        {
            SetInput(false);
        }

        KillCurrentTransition();

        if (useTransition)
        {
            currentSequence = DOTween.Sequence().SetUpdate(ignoreTimeScale);

            // Add fade tween
            var fadeTween = canvasGroup.DOFade(0f, fadeDuration);
            if (useCustomFadeCurve)
                fadeTween.SetEase(customFadeCurve);
            else
                fadeTween.SetEase(fadeEase);
            currentSequence.Join(fadeTween);

            // Add scale tween
            if (useScaleEffect)
            {
                var scaleTween = rectTransform.DOScale(startScale, fadeDuration);
                if (useCustomScaleCurve)
                    scaleTween.SetEase(customScaleCurve);
                else
                    scaleTween.SetEase(scaleEase);
                currentSequence.Join(scaleTween);
            }

            // Add position tween
            if (usePositionEffect)
            {
                var positionTween = rectTransform.DOAnchorPos(GetOffscreenPosition(), fadeDuration);
                if (useCustomPositionCurve)
                    positionTween.SetEase(customPositionCurve);
                else
                    positionTween.SetEase(positionEase);
                currentSequence.Join(positionTween);
            }

            // Setup progress callback
            currentSequence.OnUpdate(() => {
                OnTransitionProgress?.Invoke(currentSequence.ElapsedPercentage());
            });

            // Setup completion callback
            currentSequence.OnComplete(() => CompleteHide(callback));
        }
        else
        {
            canvasGroup.alpha = 0f;
            CompleteHide(callback);
        }
    }

    private void CompleteHide(Action callback = null)
    {
        gameObject.SetActive(false);
        // Tidak perlu mengaktifkan input karena UI sudah tidak aktif
        isTransitioning = false;
        Log("Hide complete.");
        OnHideComplete?.Invoke();
        callback?.Invoke();

        ProcessNextTransition();
    }

    private void ProcessNextTransition()
    {
        if (transitionQueue.Count > 0 && gameObject.activeSelf)
        {
            var nextTransition = transitionQueue.Dequeue();
            if (nextTransition.isShow)
                StartShow(nextTransition.useTransition, nextTransition.callback);
            else
                StartHide(nextTransition.useTransition, nextTransition.callback);
        }
    }

    private Vector2 GetOffscreenPosition()
    {
        Vector2 offscreenPosition = originalPosition;
        switch (slideDirection)
        {
            case UISlideDirection.Left:
                offscreenPosition.x -= slideDistance;
                break;
            case UISlideDirection.Right:
                offscreenPosition.x += slideDistance;
                break;
            case UISlideDirection.Top:
                offscreenPosition.y += slideDistance;
                break;
            case UISlideDirection.Bottom:
                offscreenPosition.y -= slideDistance;
                break;
        }
        return offscreenPosition;
    }

    public virtual void CancelTransition()
    {
        if (!isTransitioning) return;
        KillCurrentTransition();
        isTransitioning = false;
        SetInput(true);
        transitionQueue.Clear();
        Log("Transition cancelled.");
    }

    private void KillCurrentTransition()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
            currentSequence = null;
        }
    }

    public bool IsVisible() => gameObject.activeSelf && canvasGroup.alpha > 0f;
    public bool IsTransitioning() => isTransitioning;

    private void SetInput(bool state)
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = state;
            canvasGroup.blocksRaycasts = state;
        }
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
            Debug.Log($"[{gameObject.name}] {message}");
    }

    private void OnDestroy()
    {
        CancelTransition();
    }
}