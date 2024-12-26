using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class ButtonExtended : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Title("Sprite Settings")]
    [PreviewField(50, ObjectFieldAlignment.Center)]
    [LabelText("Disabled Sprite")]
    public Sprite disabledSprite;

    [PreviewField(50, ObjectFieldAlignment.Center)]
    [LabelText("Pressed Sprite")]
    public Sprite pressedSprite;

    [PreviewField(50, ObjectFieldAlignment.Center)]
    [LabelText("Highlighted Sprite")]
    public Sprite highlightedSprite;

    [Title("Opacity Settings")]
    [Range(0, 1)]
    [LabelText("Pressed Opacity")]
    public float pressedOpacity = 1f;

    [Range(0, 1)]
    [LabelText("Idle Opacity")]
    public float idleOpacity = 1f;

    [Range(0, 1)]
    [LabelText("Disabled Opacity")]
    public float disabledOpacity = 1f;

    [Range(0, 1)]
    [LabelText("Hover Opacity")]
    public float hoverOpacity = 1f;

    [Title("Delays")]
    [LabelText("Pressed Delay")]
    [MinValue(0)]
    public float pressedFirstTimeDelay;

    [LabelText("Released Delay")]
    [MinValue(0)]
    public float releasedDelay;

    [Title("Animation Settings")]
    [LabelText("Animator")]
    public Animator animator;

    [VerticalGroup("Animation Parameters")]
    [LabelText("Idle Parameter"), Tooltip("Parameter animasi untuk keadaan idle")]
    public string idleAnimationParameter = "Idle";

    [VerticalGroup("Animation Parameters")]
    [LabelText("Disabled Parameter"), Tooltip("Parameter animasi untuk keadaan disabled")]
    public string disabledAnimationParameter = "Disabled";

    [VerticalGroup("Animation Parameters")]
    [LabelText("Pressed Parameter"), Tooltip("Parameter animasi untuk keadaan pressed")]
    public string pressedAnimationParameter = "Pressed";

    [VerticalGroup("Animation Parameters")]
    [LabelText("Highlighted Parameter"), Tooltip("Parameter animasi untuk keadaan highlighted")]
    public string highlightedAnimationParameter = "Highlighted";

    private Button button;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        if (button == null || image == null)
        {
            Debug.LogError("ButtonExtended requires both a Button and an Image component.");
            enabled = false;
        }
    }

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(UpdateState);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(UpdateState);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
            Invoke(nameof(HandlePressedState), pressedFirstTimeDelay);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            SetSprite(highlightedSprite);
            SetOpacity(hoverOpacity);
            PlayAnimation(highlightedAnimationParameter);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
            ResetToIdle();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button.interactable)
            Invoke(nameof(ResetToIdle), releasedDelay);
    }

    private void UpdateState()
    {
        if (!button.interactable)
        {
            SetSprite(disabledSprite);
            SetOpacity(disabledOpacity);
            PlayAnimation(disabledAnimationParameter);
        }
        else
        {
            ResetToIdle();
        }
    }

    private void HandlePressedState()
    {
        SetSprite(pressedSprite);
        SetOpacity(pressedOpacity);
        PlayAnimation(pressedAnimationParameter);
    }

    private void ResetToIdle()
    {
        SetSprite(null); // Reset to default sprite
        SetOpacity(idleOpacity);
        PlayAnimation(idleAnimationParameter);
    }

    private void SetSprite(Sprite sprite)
    {
        image.sprite = sprite != null ? sprite : button.image.sprite;
    }

    private void SetOpacity(float opacity)
    {
        var color = image.color;
        color.a = opacity;
        image.color = color;
    }

    private void PlayAnimation(string parameter)
    {
        if (animator != null && !string.IsNullOrEmpty(parameter))
            animator.SetTrigger(parameter);
    }
}
