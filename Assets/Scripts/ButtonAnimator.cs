using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    public float hoverScale = 1.1f;
    public float pressScale = 0.95f;
    public float animationSpeed = 10f;

    [Header("Sound")]
    public bool playSoundOnClick = true;
    public bool playSoundOnHover = false;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Button button;
    private bool isHovering;
    private bool isPressed;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        // Smooth scale animation
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            isHovering = true;
            targetScale = originalScale * hoverScale;

            if (playSoundOnHover && AudioManager.Instance != null)
            {
                // Play subtle hover sound
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (!isPressed)
        {
            targetScale = originalScale;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            isPressed = true;
            targetScale = originalScale * pressScale;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        
        if (button != null && button.interactable)
        {
            if (playSoundOnClick && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            if (isHovering)
            {
                targetScale = originalScale * hoverScale;
            }
            else
            {
                targetScale = originalScale;
            }
        }
    }

    private void OnDisable()
    {
        // Reset scale when disabled
        transform.localScale = originalScale;
        targetScale = originalScale;
        isHovering = false;
        isPressed = false;
    }
}

