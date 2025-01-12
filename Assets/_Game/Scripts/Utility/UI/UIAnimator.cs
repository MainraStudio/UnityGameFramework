using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    public InitialRun initialRun = InitialRun.None;
    public AnimationType animationType;
    public LoopType loopType = LoopType.Yoyo;
    public float duration = 0.3f;
    public float intensity = 1f;
    public float frequency = 2f;
    public float startTime = 0f;
    public AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
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

    private void FadeIn()
    {
        ResetAnimation();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, duration)
            .SetDelay(startTime)
            .SetEase(Ease.InOutQuad);
    }

    private void FadeOut()
    {
        ResetAnimation();
        canvasGroup.DOFade(0, duration)
            .SetDelay(startTime)
            .SetEase(Ease.InOutQuad);
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
        canvasGroup.transform.localPosition = startPosition;
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
        canvasGroup.transform.localScale = Vector3.zero;
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