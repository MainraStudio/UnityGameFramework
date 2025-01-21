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
    private List<Tween> activeTweens = new List<Tween>();
    private Coroutine sequentialCoroutine;

    private void Start()
    {
        if (startFromInitialActiveState)
        {
            RunSimultaneousTweens();
        }
    }

    private Tween RunTween(TweenSettings tween, GameObject defaultTarget)
    {
        GameObject target = tween.target != null ? tween.target : defaultTarget;
        if (target == null)
        {
            Debug.LogWarning($"Target for tween {tween.name} is null. Skipping.");
            return null;
        }

        Tween tweenAction = null;
        switch (tween.tweenType)
        {
            case TweenSettings.TweenType.Move:
                tweenAction = RunMoveTween(tween, target);
                break;
            case TweenSettings.TweenType.Scale:
                tweenAction = RunScaleTween(tween, target);
                break;
            case TweenSettings.TweenType.Rotate:
                tweenAction = RunRotateTween(tween, target);
                break;
            case TweenSettings.TweenType.Fade:
                tweenAction = RunFadeTween(tween, target);
                break;
            case TweenSettings.TweenType.Color:
                tweenAction = RunColorTween(tween, target);
                break;
        }

        if (tweenAction != null)
        {
            activeTweens.Add(tweenAction);
        }

        return tweenAction;
    }

    private Tween RunMoveTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction = target.TryGetComponent(out RectTransform rectTransform)
            ? rectTransform.DOAnchorPos((Vector2)tween.targetValue, tween.duration)
            : target.transform.DOMove(tween.targetValue, tween.duration);

        ApplyTweenSettings(tween, tweenAction);
        return tweenAction;
    }

    private Tween RunScaleTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction = target.TryGetComponent(out RectTransform rectTransform)
            ? rectTransform.DOScale(tween.targetValue, tween.duration)
            : target.transform.DOScale(tween.targetValue, tween.duration);

        ApplyTweenSettings(tween, tweenAction);
        return tweenAction;
    }

    private Tween RunRotateTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction = target.transform.DORotate(tween.targetValue, tween.duration);
        ApplyTweenSettings(tween, tweenAction);
        return tweenAction;
    }

    private Tween RunFadeTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction = null;

        if (target.TryGetComponent(out CanvasGroup canvasGroup))
        {
            tweenAction = canvasGroup.DOFade(tween.targetAlpha, tween.duration);
        }
        else if (target.TryGetComponent(out Image image))
        {
            tweenAction = image.DOFade(tween.targetAlpha, tween.duration);
        }
        else if (target.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            tweenAction = spriteRenderer.DOFade(tween.targetAlpha, tween.duration);
        }
        else if (target.TryGetComponent(out TMPro.TMP_Text tmpText))
        {
            tweenAction = tmpText.DOFade(tween.targetAlpha, tween.duration);
        }

        if (tweenAction != null)
        {
            ApplyTweenSettings(tween, tweenAction);
        }

        return tweenAction;
    }

    private Tween RunColorTween(TweenSettings tween, GameObject target)
    {
        Tween tweenAction = null;

        if (target.TryGetComponent(out Renderer renderer))
        {
            tweenAction = renderer.material.DOColor(tween.targetColor, tween.duration);
        }
        else if (target.TryGetComponent(out Image image))
        {
            tweenAction = image.DOColor(tween.targetColor, tween.duration);
        }
        else if (target.TryGetComponent(out TMPro.TMP_Text tmpText))
        {
            tweenAction = tmpText.DOColor(tween.targetColor, tween.duration);
        }
        else if (target.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            tweenAction = spriteRenderer.DOColor(tween.targetColor, tween.duration);
        }

        if (tweenAction != null)
        {
            ApplyTweenSettings(tween, tweenAction);
        }

        return tweenAction;
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
            .SetLoops(tween.loop ? (tween.loopCount > 0 ? tween.loopCount : -1) : 1,
                tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart)
            .OnComplete(() =>
            {
                OnTweenComplete(tween);
                activeTweens.Remove(tweenAction);
                if (--activeSimultaneousTweens <= 0 && sequentialCoroutine == null)
                {
                    RunSequentialTweens();
                }
            });

        if (useUnscaledTime)
        {
            tweenAction.SetUpdate(true);
        }

        activeSimultaneousTweens++;
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
        if (sequentialCoroutine != null)
        {
            StopCoroutine(sequentialCoroutine);
        }
        sequentialCoroutine = StartCoroutine(RunSequentialTweensCoroutine());
    }

    private IEnumerator RunSequentialTweensCoroutine()
    {
        foreach (var tween in sequentialTweens)
        {
            Tween tweenAction = RunTween(tween, gameObject);
            if (tweenAction != null)
            {
                yield return tweenAction.WaitForCompletion();
            }
        }
        sequentialCoroutine = null;
    }

    public void CancelAllTweens()
    {
        foreach (var tween in activeTweens)
        {
            tween.Kill();
        }
        activeTweens.Clear();
    }

    public void PauseAllTweens()
    {
        foreach (var tween in activeTweens)
        {
            tween.Pause();
        }
    }

    public void ResumeAllTweens()
    {
        foreach (var tween in activeTweens)
        {
            tween.Play();
        }
    }
}
