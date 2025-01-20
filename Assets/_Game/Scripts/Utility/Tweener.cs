using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class Tweener : MonoBehaviour
{
    [System.Serializable]
    public class TweenSettings
    {
        public string name;
        public enum TweenType
        {
            Move,
            Scale,
            Rotate,
            Fade,
            Color
        }

        public TweenType tweenType;
        public Vector3 targetValue;
        public float duration;
        public Ease easeType;
        public float delay;
        public bool loop;
        public int loopCount;
        public bool pingpong;
        public float targetAlpha;
        public Color targetColor;
        public GameObject target;
        public UnityEngine.Events.UnityEvent OnTweenComplete;
        public AnimationCurve customCurve;
        public bool useCustomCurve;
    }

    [Header("General Settings")]
    [Tooltip("Use unscaled time for tweens")]
    public bool useUnscaledTime = false;

    [Tooltip("Start tween from the initial active state of the object")]
    public bool startFromInitialActiveState = true;

    [Header("Tweens")]
    public List<TweenSettings> simultaneousTweens = new List<TweenSettings>();
    public List<TweenSettings> sequentialTweens = new List<TweenSettings>();

    private int activeSimultaneousTweens = 0;

    private void OnEnable()
    {
        if (startFromInitialActiveState)
        {
            RunSimultaneousTweens();
        }
    }

    private void Start()
    {
        if (startFromInitialActiveState)
        {
            RunSimultaneousTweens();
        }
    }

    private void RunTween(TweenSettings tween, GameObject defaultTarget)
    {
        GameObject target = tween.target != null ? tween.target : defaultTarget;

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
            case TweenSettings.TweenType.Fade:
                RunFadeTween(tween, target);
                break;
            case TweenSettings.TweenType.Color:
                RunColorTween(tween, target);
                break;
        }
    }

    private void RunMoveTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction;
        if (target.TryGetComponent(out RectTransform rectTransform))
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
        if (target.TryGetComponent(out RectTransform rectTransform))
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
        if (target.TryGetComponent(out RectTransform rectTransform))
        {
            tweenAction = rectTransform.DORotate(tween.targetValue, tween.duration);
        }
        else
        {
            tweenAction = target.transform.DORotate(tween.targetValue, tween.duration);
        }

        ApplyTweenSettings(tween, tweenAction);
    }

    private void RunFadeTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction = null;

        if (target.TryGetComponent(out RectTransform rectTransform))
        {
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Image image = target.GetComponent<Image>();
                if (image != null)
                {
                    tweenAction = image.DOFade(tween.targetAlpha, tween.duration);
                }
            }
            else
            {
                tweenAction = canvasGroup.DOFade(tween.targetAlpha, tween.duration);
            }
        }
        else
        {
            SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                tweenAction = spriteRenderer.DOFade(tween.targetAlpha, tween.duration);
            }
        }

        if (tweenAction != null)
        {
            ApplyTweenSettings(tween, tweenAction);
        }
    }

    private void RunColorTween(TweenSettings tween, GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            Tween tweenAction = renderer.material.DOColor(tween.targetColor, tween.duration);
            ApplyTweenSettings(tween, tweenAction);
        }
        else
        {
            Image image = target.GetComponent<Image>();
            if (image != null)
            {
                Tween tweenAction = image.DOColor(tween.targetColor, tween.duration);
                ApplyTweenSettings(tween, tweenAction);
            }
        }
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

        activeSimultaneousTweens++;
        tweenAction.OnComplete(() =>
        {
            activeSimultaneousTweens--;
            if (activeSimultaneousTweens == 0)
            {
                RunSequentialTweens();
            }
        });
    }

    private void OnTweenComplete(TweenSettings tween)
    {
        tween.OnTweenComplete?.Invoke();
    }

    public void RunSimultaneousTweens()
    {
        foreach (var tween in simultaneousTweens)
        {
            RunTween(tween, gameObject);
        }
    }

    public void RunSequentialTweens()
    {
        StartCoroutine(RunSequentialTweensCoroutine());
    }

    private IEnumerator RunSequentialTweensCoroutine()
    {
        foreach (var tween in sequentialTweens)
        {
            RunTween(tween, gameObject);
            yield return new WaitForSeconds(tween.duration + tween.delay);
        }
    }
}