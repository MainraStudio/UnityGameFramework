using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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

    [BoxGroup("Global Settings")]
    [Tooltip("Apakah animasi menggunakan unscaled time.")]
    public bool useUnscaledTime = false;

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
            Debug.Log($"Starting {tween.tweenType} tween on {target.name}");
        }

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
                    Debug.LogWarning($"Failed to execute {tween.tweenType} tween on {target.name}");
                }
                break;
        }
    }

    private void RunMoveTween(TweenSettings tween, GameObject target)
    {
        var tweenAction = target.transform.DOMove(tween.targetValue, tween.duration)
            .SetEase(tween.easeType)
            .SetDelay(tween.delay)
            .OnComplete(() => OnTweenComplete(tween))
            .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);

        if (useUnscaledTime)
        {
            tweenAction.SetUpdate(true);
        }
    }

    private void RunScaleTween(TweenSettings tween, GameObject target)
    {
        var tweenAction = target.transform.DOScale(tween.targetValue, tween.duration)
            .SetEase(tween.easeType)
            .SetDelay(tween.delay)
            .OnComplete(() => OnTweenComplete(tween))
            .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);

        if (useUnscaledTime)
        {
            tweenAction.SetUpdate(true);
        }
    }

    private void RunRotateTween(TweenSettings tween, GameObject target)
    {
        var tweenAction = target.transform.DORotate(tween.targetValue, tween.duration)
            .SetEase(tween.easeType)
            .SetDelay(tween.delay)
            .OnComplete(() => OnTweenComplete(tween))
            .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);

        if (useUnscaledTime)
        {
            tweenAction.SetUpdate(true);
        }
    }

    private void RunFadeTween(TweenSettings tween, GameObject target)
    {
        if (target.TryGetComponent(out CanvasGroup canvasGroup))
        {
            var tweenAction = canvasGroup.DOFade(tween.targetAlpha, tween.duration)
                .SetEase(tween.easeType)
                .SetDelay(tween.delay)
                .OnComplete(() => OnTweenComplete(tween))
                .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);

            if (useUnscaledTime)
            {
                tweenAction.SetUpdate(true);
            }
        }
        else
        {
            Debug.LogWarning($"Failed to execute Fade tween on {target.name}");
        }
    }

    private void RunColorTween(TweenSettings tween, GameObject target)
    {
        if (tween.isUIElement && target.TryGetComponent(out Graphic graphic))
        {
            var tweenAction = graphic.DOColor(tween.targetColor, tween.duration)
                .SetEase(tween.easeType)
                .SetDelay(tween.delay)
                .OnComplete(() => OnTweenComplete(tween))
                .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);

            if (useUnscaledTime)
            {
                tweenAction.SetUpdate(true);
            }
        }
        else if (target.TryGetComponent(out Renderer renderer))
        {
            var tweenAction = renderer.material.DOColor(tween.targetColor, tween.duration)
                .SetEase(tween.easeType)
                .SetDelay(tween.delay)
                .OnComplete(() => OnTweenComplete(tween))
                .SetLoops(tween.loop ? -1 : tween.loopCount, tween.pingpong ? DG.Tweening.LoopType.Yoyo : DG.Tweening.LoopType.Restart);

            if (useUnscaledTime)
            {
                tweenAction.SetUpdate(true);
            }
        }
        else
        {
            Debug.LogWarning($"Failed to execute Color tween on {target.name}");
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
}