using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class Tweener : MonoBehaviour
{
    [System.Serializable]
    public class TweenSettings
    {
        public enum TweenType { Move, Scale, Rotate, Fade, Color }

        [BoxGroup("Type")]
        [Tooltip("Jenis animasi yang akan dijalankan.")]
        public TweenType tweenType;

        [BoxGroup("Target")]
        [Tooltip("GameObject target animasi (default adalah GameObject ini).")]
        public GameObject targetObject;

        [BoxGroup("Target")]
        [Tooltip("Apakah target adalah elemen UI.")]
        public bool isUIElement = false;

        [BoxGroup("Values")]
        [ShowIf("")]
        [Tooltip("Nilai target untuk Move, Scale, atau Rotate.")]
        public Vector3 targetValue;

        [BoxGroup("Values")]
        [Tooltip("Alpha target untuk animasi Fade (0 = transparan, 1 = sepenuhnya terlihat).")]
        [Range(0f, 1f)] public float targetAlpha = 1f;

        [BoxGroup("Values")]
        [Tooltip("Warna target untuk animasi Color.")]
        public Color targetColor = Color.white;

        [BoxGroup("Timing")]
        [Tooltip("Durasi animasi dalam detik.")]
        [Min(0f)] public float duration = 1f;

        [BoxGroup("Timing")]
        [Tooltip("Delay sebelum animasi dimulai.")]
        [Min(0f)] public float delay = 0f;

        [BoxGroup("Easing")]
        [Tooltip("Jenis easing yang akan digunakan.")]
        public Ease easeType = Ease.Linear;

        [BoxGroup("Looping")]
        [Tooltip("Apakah animasi harus diulang?")]
        public bool loop = false;

        [BoxGroup("Looping")]
        [Tooltip("Apakah animasi harus pingpong?")]
        public bool pingpong = false;

        [BoxGroup("Looping")]
        [Tooltip("Jumlah loop animasi (0 untuk tidak terbatas).")]
        public int loopCount = 0;

        [BoxGroup("Events")]
        [Tooltip("Callback saat animasi selesai.")]
        public UnityEvent onComplete;

        // Kondisi untuk NaughtyAttributes
        private bool RequiresVectorValue => tweenType == TweenType.Move || tweenType == TweenType.Scale || tweenType == TweenType.Rotate;
        private bool IsFadeType => tweenType == TweenType.Fade;
        private bool IsColorType => tweenType == TweenType.Color;
    }

    [BoxGroup("Global Settings")]
    [Tooltip("Apakah mode debug aktif? Jika ya, log akan ditampilkan di console.")]
    public bool debugMode = false;

    [BoxGroup("Global Settings")]
    [Tooltip("Apakah animasi dijalankan saat start.")]
    public bool runOnStart = true;

    [ReorderableList]
    [BoxGroup("Tween Animations")]
    [Tooltip("List animasi yang akan dijalankan secara berurutan.")]
    public List<TweenSettings> tweenAnimations = new List<TweenSettings>();

    private void Start()
    {
        if (runOnStart)
        {
            StartCoroutine(ExecuteAnimations());
        }
    }

    private IEnumerator ExecuteAnimations()
    {
        foreach (var tween in tweenAnimations)
        {
            RunTweenAnimation(tween);
            yield return new WaitUntil(() => !DOTween.IsTweening(tween.targetObject));
        }

        if (debugMode)
        {
            Debug.Log("All animations completed.");
        }
    }

    private void RunTweenAnimation(TweenSettings tween)
    {
        GameObject target = tween.targetObject != null ? tween.targetObject : gameObject;

        if (debugMode)
        {
            DebugAnimationStart(tween.tweenType, target);
        }

        if (tween.isUIElement)
        {
            RunUITween(tween, target);
        }
        else
        {
            RunNonUITween(tween, target);
        }
    }

    private void RunUITween(TweenSettings tween, GameObject target)
    {
        switch (tween.tweenType)
        {
            case TweenSettings.TweenType.Move:
                RunMoveTweenUI(tween, target);
                break;
            case TweenSettings.TweenType.Scale:
                RunScaleTweenUI(tween, target);
                break;
            case TweenSettings.TweenType.Rotate:
                RunRotateTweenUI(tween, target);
                break;
            case TweenSettings.TweenType.Fade:
                RunFadeTween(tween, target);
                break;
            case TweenSettings.TweenType.Color:
                RunColorTween(tween, target);
                break;
            default:
                if (debugMode)
                {
                    DebugAnimationFailed(tween.tweenType, target);
                }
                break;
        }
    }

    private void RunNonUITween(TweenSettings tween, GameObject target)
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
            case TweenSettings.TweenType.Fade:
                RunFadeTween(tween, target);
                break;
            case TweenSettings.TweenType.Color:
                RunColorTween(tween, target);
                break;
            default:
                if (debugMode)
                {
                    DebugAnimationFailed(tween.tweenType, target);
                }
                break;
        }
    }

    private void RunMoveTween(TweenSettings tween, GameObject target)
    {
        target.transform.DOMove(tween.targetValue, tween.duration)
            .SetEase(tween.easeType)
            .SetDelay(tween.delay)
            .OnComplete(() => OnTweenComplete(tween))
            .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
    }

    private void RunScaleTween(TweenSettings tween, GameObject target)
    {
        target.transform.DOScale(tween.targetValue, tween.duration)
            .SetEase(tween.easeType)
            .SetDelay(tween.delay)
            .OnComplete(() => OnTweenComplete(tween))
            .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
    }

    private void RunRotateTween(TweenSettings tween, GameObject target)
    {
        target.transform.DORotate(tween.targetValue, tween.duration)
            .SetEase(tween.easeType)
            .SetDelay(tween.delay)
            .OnComplete(() => OnTweenComplete(tween))
            .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
    }

    private void RunMoveTweenUI(TweenSettings tween, GameObject target)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.DOAnchorPos(tween.targetValue, tween.duration)
                .SetEase(tween.easeType)
                .SetDelay(tween.delay)
                .OnComplete(() => OnTweenComplete(tween))
                .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
        }
        else
        {
            DebugAnimationFailed(TweenSettings.TweenType.Move, target);
        }
    }

    private void RunScaleTweenUI(TweenSettings tween, GameObject target)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.DOScale(tween.targetValue, tween.duration)
                .SetEase(tween.easeType)
                .SetDelay(tween.delay)
                .OnComplete(() => OnTweenComplete(tween))
                .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
        }
        else
        {
            DebugAnimationFailed(TweenSettings.TweenType.Scale, target);
        }
    }

    private void RunRotateTweenUI(TweenSettings tween, GameObject target)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.DORotate(tween.targetValue, tween.duration)
                .SetEase(tween.easeType)
                .SetDelay(tween.delay)
                .OnComplete(() => OnTweenComplete(tween))
                .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
        }
        else
        {
            DebugAnimationFailed(TweenSettings.TweenType.Rotate, target);
        }
    }

    private void RunFadeTween(TweenSettings tween, GameObject target)
    {
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(tween.targetAlpha, tween.duration)
                .SetEase(tween.easeType)
                .SetDelay(tween.delay)
                .OnComplete(() => OnTweenComplete(tween))
                .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
        }
        else
        {
            DebugAnimationFailed(TweenSettings.TweenType.Fade, target);
        }
    }

    private void RunColorTween(TweenSettings tween, GameObject target)
    {
        if (tween.isUIElement)
        {
            if (target.TryGetComponent(out Graphic graphic))
            {
                graphic.DOColor(tween.targetColor, tween.duration)
                    .SetEase(tween.easeType)
                    .SetDelay(tween.delay)
                    .OnComplete(() => OnTweenComplete(tween))
                    .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
            }
        }
        else if (target.TryGetComponent(out Renderer renderer))
        {
            renderer.material.DOColor(tween.targetColor, tween.duration)
                .SetEase(tween.easeType)
                .SetDelay(tween.delay)
                .OnComplete(() => OnTweenComplete(tween))
                .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);
        }
        else
        {
            DebugAnimationFailed(TweenSettings.TweenType.Color, target);
        }
    }

    private void OnTweenComplete(TweenSettings tween)
    {
        tween.onComplete?.Invoke();
        if (debugMode)
        {
            Debug.Log($"Tween {tween.tweenType} completed for {tween.targetObject.name}");
        }
    }

    private void DebugAnimationStart(TweenSettings.TweenType type, GameObject target)
    {
        if (debugMode)
        {
            Debug.Log($"Starting {type} tween on {target.name}");
        }
    }

    private void DebugAnimationFailed(TweenSettings.TweenType type, GameObject target)
    {
        if (debugMode)
        {
            Debug.LogWarning($"Failed to execute {type} tween on {target.name}");
        }
    }
}