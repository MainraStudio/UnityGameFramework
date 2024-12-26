using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    [Title("General Settings")]
    [Tooltip("Choose whether the animation runs initially or manually triggered.")]
    [EnumToggleButtons]
    public InitialRun initialRun = InitialRun.None;

    [Tooltip("Choose the type of animation you want to apply.")]
    [EnumToggleButtons]
    public AnimationType animationType;

    [Tooltip("Enable looping for animations.")]
    [ShowIf("@animationType != AnimationType.None")]
    [EnumToggleButtons]
    public LoopType loopType = LoopType.Yoyo;

    [FoldoutGroup("Animation Parameters")]
    [MinValue(0.1f), MaxValue(5f), Tooltip("Duration of the animation in seconds.")]
    [ShowIf("@animationType != AnimationType.None")]
    public float duration = 0.3f;

    [FoldoutGroup("Animation Parameters")]
    [MinValue(0f), MaxValue(10f), Tooltip("Intensity of the animation.")]
    [ShowIf("ShowIntensity")]
    public float intensity = 1f;

    [FoldoutGroup("Animation Parameters")]
    [MinValue(0.1f), MaxValue(5f), Tooltip("Frequency for oscillating animations.")]
    [ShowIf("ShowFrequency")]
    public float frequency = 2f;

    [FoldoutGroup("Animation Parameters")]
    [MinValue(0f), MaxValue(10f), Tooltip("Start time delay before animation begins.")]
    [ShowIf("@animationType != AnimationType.None")]
    public float startTime = 0f;

    [FoldoutGroup("Animation Parameters")]
    [Tooltip("Custom curve for animation easing.")]
    [ShowIf("@animationType != AnimationType.None")]
    public AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [FoldoutGroup("Animation Parameters")]
    [Tooltip("Direction of the animation.")]
    [ShowIf("ShowDirection")]
    public Direction direction = Direction.Left;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (initialRun == InitialRun.OnStart)
        {
            Invoke(nameof(PlayAnimation), startTime);
        }
    }

    [Button("Play Animation", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
    [Tooltip("Click to play the selected animation.")]
    public void PlayAnimation()
    {
        ResetAnimation();

        switch (animationType)
        {
            case AnimationType.None:
                Debug.Log("No animation selected.");
                break;
            case AnimationType.Bounce:
                PlayBounce();
                break;
            case AnimationType.Shake:
                PlayShake();
                break;
            case AnimationType.Pulse:
                PlayPulse();
                break;
            case AnimationType.Rotate:
                PlayRotate();
                break;
            case AnimationType.Swing:
                PlaySwing();
                break;
            case AnimationType.Throb:
                PlayThrob();
                break;
            case AnimationType.Wiggle:
                PlayWiggle();
                break;
            case AnimationType.FadeIn:
                FadeIn();
                break;
            case AnimationType.FadeOut:
                FadeOut();
                break;
            case AnimationType.SlideIn:
                SlideIn();
                break;
            case AnimationType.SlideOut:
                SlideOut();
                break;
            case AnimationType.ZoomIn:
                ZoomIn();
                break;
            case AnimationType.ZoomOut:
                ZoomOut();
                break;
            case AnimationType.Flip:
                Flip();
                break;
            default:
                Debug.LogWarning("Animation type not implemented yet!");
                break;
        }
    }

    [Button("Reset Animation", ButtonSizes.Medium), GUIColor(1f, 0.5f, 0.5f)]
    [Tooltip("Click to reset the animation.")]
    public void ResetAnimation()
    {
        canvasGroup.DOKill();
        canvasGroup.transform.DOKill();
        canvasGroup.alpha = 1;
        canvasGroup.transform.localScale = Vector3.one;
        canvasGroup.transform.rotation = Quaternion.identity;
        canvasGroup.transform.localPosition = Vector3.zero;
    }

    private void PlayBounce()
    {
        canvasGroup.transform.DOScale(Vector3.one * intensity, duration)
            .SetDelay(startTime)
            .SetLoops(loopType == LoopType.None ? 0 : -1, (DG.Tweening.LoopType)loopType)
            .SetEase(animationCurve);
    }

    private void PlayShake()
    {
        canvasGroup.transform.DOShakePosition(duration, intensity, Mathf.RoundToInt(frequency), 90, false, true)
            .SetDelay(startTime)
            .SetLoops(loopType == LoopType.None ? 0 : -1, (DG.Tweening.LoopType)loopType);
    }

    private void PlayPulse()
    {
        canvasGroup.DOFade(1 - intensity, duration / 2)
            .SetDelay(startTime)
            .SetLoops(loopType == LoopType.None ? 0 : -1, DG.Tweening.LoopType.Yoyo)
            .SetEase(animationCurve);
    }

    private void PlayRotate()
    {
        canvasGroup.transform.DORotate(new Vector3(0, 0, 360) * intensity, duration, RotateMode.FastBeyond360)
            .SetDelay(startTime)
            .SetLoops(loopType == LoopType.None ? 0 : -1, (DG.Tweening.LoopType)loopType)
            .SetEase(animationCurve);
    }

    private void PlaySwing()
    {
        canvasGroup.transform.DORotate(new Vector3(0, 0, intensity * 15f), duration / 2, RotateMode.Fast)
            .SetLoops(loopType == LoopType.None ? 0 : -1, DG.Tweening.LoopType.Yoyo)
            .SetDelay(startTime)
            .SetEase(animationCurve);
    }

    private void PlayThrob()
    {
        canvasGroup.transform.DOScale(Vector3.one + Vector3.one * (intensity * 0.1f), duration / 2)
            .SetLoops(loopType == LoopType.None ? 0 : -1, DG.Tweening.LoopType.Yoyo)
            .SetDelay(startTime)
            .SetEase(animationCurve);
    }

    private void PlayWiggle()
    {
        canvasGroup.transform.DOShakeRotation(duration, intensity * 15f, Mathf.RoundToInt(frequency), 90, false)
            .SetLoops(loopType == LoopType.None ? 0 : -1, (DG.Tweening.LoopType)loopType)
            .SetDelay(startTime);
    }

    [FoldoutGroup("Utility Functions"), Button("Fade In", ButtonSizes.Medium), GUIColor(0.6f, 1f, 0.6f)]
    [Tooltip("Click to fade in the UI element.")]
    private void FadeIn()
    {
        ResetAnimation();
        canvasGroup.alpha = 0; // Set initial alpha to 0
        canvasGroup.DOFade(1, duration)
            .SetDelay(startTime)
            .SetEase(Ease.InOutQuad); // Use a smoother easing function
    }

    [FoldoutGroup("Utility Functions"), Button("Fade Out", ButtonSizes.Medium), GUIColor(1f, 0.6f, 0.6f)]
    [Tooltip("Click to fade out the UI element.")]
    private void FadeOut()
    {
        ResetAnimation();
        canvasGroup.DOFade(0, duration)
            .SetDelay(startTime)
            .SetEase(Ease.InOutQuad); // Use a smoother easing function
    }

    private void SlideIn()
    {
        ResetAnimation();
        Vector3 startPosition = direction switch
        {
            Direction.Left => new Vector3(-Screen.width, 0, 0),
            Direction.Right => new Vector3(Screen.width, 0, 0),
            Direction.Up => new Vector3(0, Screen.height, 0),
            Direction.Down => new Vector3(0, -Screen.height, 0),
            _ => Vector3.zero
        };
        canvasGroup.transform.localPosition = startPosition; // Start off-screen
        canvasGroup.transform.DOLocalMove(Vector3.zero, duration)
            .SetDelay(startTime)
            .SetEase(animationCurve);
    }

    private void SlideOut()
    {
        ResetAnimation();
        Vector3 endPosition = direction switch
        {
            Direction.Left => new Vector3(-Screen.width, 0, 0),
            Direction.Right => new Vector3(Screen.width, 0, 0),
            Direction.Up => new Vector3(0, Screen.height, 0),
            Direction.Down => new Vector3(0, -Screen.height, 0),
            _ => Vector3.zero
        };
        canvasGroup.transform.DOLocalMove(endPosition, duration)
            .SetDelay(startTime)
            .SetEase(animationCurve);
    }

    private void ZoomIn()
    {
        ResetAnimation();
        canvasGroup.transform.localScale = Vector3.zero; // Start from zero scale
        canvasGroup.transform.DOScale(Vector3.one, duration)
            .SetDelay(startTime)
            .SetEase(animationCurve);
    }

    private void ZoomOut()
    {
        ResetAnimation();
        canvasGroup.transform.DOScale(Vector3.zero, duration)
            .SetDelay(startTime)
            .SetEase(animationCurve);
    }

    private void Flip()
    {
        ResetAnimation();
        canvasGroup.transform.DORotate(new Vector3(0, 180, 0), duration, RotateMode.FastBeyond360)
            .SetDelay(startTime)
            .SetLoops(loopType == LoopType.None ? 0 : -1, (DG.Tweening.LoopType)loopType)
            .SetEase(animationCurve);
    }

    private bool ShowIntensity()
    {
        return animationType == AnimationType.Bounce || animationType == AnimationType.Shake ||
               animationType == AnimationType.Pulse || animationType == AnimationType.Rotate ||
               animationType == AnimationType.Swing || animationType == AnimationType.Throb ||
               animationType == AnimationType.Wiggle || animationType == AnimationType.ZoomIn ||
               animationType == AnimationType.ZoomOut || animationType == AnimationType.Flip;
    }

    private bool ShowFrequency()
    {
        return animationType == AnimationType.Shake || animationType == AnimationType.Wiggle;
    }

    private bool ShowDirection()
    {
        return animationType == AnimationType.SlideIn || animationType == AnimationType.SlideOut;
    }
}

public enum AnimationType
{
    None,
    Bounce,
    Shake,
    Pulse,
    Rotate,
    Swing,
    Throb,
    Wiggle,
    FadeIn,
    FadeOut,
    SlideIn,
    SlideOut,
    ZoomIn,
    ZoomOut,
    Flip
}

public enum InitialRun
{
    None,
    OnStart
}

public enum LoopType
{
    None,
    Restart,
    Yoyo,
    Incremental
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}