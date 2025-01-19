using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class Tweener : MonoBehaviour
{
    [System.Serializable]
    public class TweenSettings
    {
        public enum TweenType
        {
            Move,
            Scale,
            Rotate,
            Fade,
            Color
        }

        public TweenType tweenType;
        public bool isUIElement;
        public Vector3 targetValue;
        public float duration;
        public Ease easeType;
        public float delay;
        public bool loop;
        public int loopCount;
        public bool pingpong;
        public float targetAlpha; // For Fade
        public Color targetColor; // For Color
        public bool startOnActive; // New property
        public UnityEngine.Events.UnityEvent OnTweenComplete; // New property
        public AnimationCurve customCurve; // New property
        public bool useCustomCurve; // New property
    }

    public bool useUnscaledTime = false;
    public List<TweenSettings> tweenAnimations = new List<TweenSettings>();

    private void OnEnable()
    {
        foreach (var tween in tweenAnimations)
        {
            if (tween.startOnActive)
            {
                RunTween(tween, gameObject);
            }
        }
    }

    private void Start()
    {
        foreach (var tween in tweenAnimations)
        {
            if (!tween.startOnActive)
            {
                RunTween(tween, gameObject);
            }
        }
    }

    private void RunTween(TweenSettings tween, GameObject target)
    {
        switch (tween.tweenType)
        {
            case TweenSettings.TweenType.Move:
                RunMoveTween(tween, target);
                break;
            case TweenSettings.TweenType.Scale:
                RunScaleTween(tween, target);
                break;
            case TweenSettings.TweenType.Rotate:
                RunRotateTween(tween, target);
                break;
            // Add cases for Fade and Color if needed
        }
    }

    private void RunMoveTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction;
        if (tween.isUIElement && target.TryGetComponent(out RectTransform rectTransform))
        {
            tweenAction = rectTransform.DOAnchorPos((Vector2)tween.targetValue, tween.duration);
        }
        else
        {
            tweenAction = target.transform.DOMove(tween.targetValue, tween.duration);
        }

        ApplyTweenSettings(tween, tweenAction);
    }

    private void RunScaleTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction;
        if (tween.isUIElement && target.TryGetComponent(out RectTransform rectTransform))
        {
            tweenAction = rectTransform.DOScale(tween.targetValue, tween.duration);
        }
        else
        {
            tweenAction = target.transform.DOScale(tween.targetValue, tween.duration);
        }

        ApplyTweenSettings(tween, tweenAction);
    }

    private void RunRotateTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction;
        if (tween.isUIElement && target.TryGetComponent(out RectTransform rectTransform))
        {
            tweenAction = rectTransform.DORotate(tween.targetValue, tween.duration);
        }
        else
        {
            tweenAction = target.transform.DORotate(tween.targetValue, tween.duration);
        }

        ApplyTweenSettings(tween, tweenAction);
    }

    private void ApplyTweenSettings(TweenSettings tween, Tween tweenAction)
    {
        if (tween.useCustomCurve && tween.customCurve != null)
        {
            tweenAction.SetEase(tween.customCurve);
        }
        else
        {
            tweenAction.SetEase(tween.easeType);
        }

        tweenAction.SetDelay(tween.delay)
            .OnComplete(() => OnTweenComplete(tween))
            .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);

        if (useUnscaledTime)
        {
            tweenAction.SetUpdate(true);
        }
    }

    private void OnTweenComplete(TweenSettings tween)
    {
        tween.OnTweenComplete?.Invoke();
    }
}