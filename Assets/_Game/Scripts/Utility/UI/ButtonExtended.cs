using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class ButtonExtended : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Sprite disabledSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite highlightedSprite;

    [SerializeField] [Range(0, 1)] private float pressedOpacity = 1f;
    [SerializeField] [Range(0, 1)] private float idleOpacity = 1f;
    [SerializeField] [Range(0, 1)] private float disabledOpacity = 1f;
    [SerializeField] [Range(0, 1)] private float hoverOpacity = 1f;

    [SerializeField] [Min(0)] private float pressedFirstTimeDelay;
    [SerializeField] [Min(0)] private float releasedDelay;

    [SerializeField] private Animator animator;
    [SerializeField] private string idleAnimationParameter = "Idle";
    [SerializeField] private string disabledAnimationParameter = "Disabled";
    [SerializeField] private string pressedAnimationParameter = "Pressed";
    [SerializeField] private string highlightedAnimationParameter = "Highlighted";

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