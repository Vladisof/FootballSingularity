using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NotificationUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI messageText;
    public Image backgroundImage;
    public Image iconImage;

    [Header("Icons")]
    public Sprite infoIcon;
    public Sprite successIcon;
    public Sprite warningIcon;
    public Sprite errorIcon;

    [Header("Colors")]
    public Color infoColor = new Color(0.2f, 0.6f, 1f);
    public Color successColor = new Color(0.2f, 0.8f, 0.2f);
    public Color warningColor = new Color(1f, 0.8f, 0.2f);
    public Color errorColor = new Color(1f, 0.2f, 0.2f);

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Setup(string message, NotificationType type, float duration)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }

        // Set colors and icons based on type
        Color typeColor;
        Sprite typeIcon;

        switch (type)
        {
            case NotificationType.Success:
                typeColor = successColor;
                typeIcon = successIcon;
                break;
            case NotificationType.Warning:
                typeColor = warningColor;
                typeIcon = warningIcon;
                break;
            case NotificationType.Error:
                typeColor = errorColor;
                typeIcon = errorIcon;
                break;
            case NotificationType.Info:
            default:
                typeColor = infoColor;
                typeIcon = infoIcon;
                break;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = typeColor;
        }

        if (iconImage != null && typeIcon != null)
        {
            iconImage.sprite = typeIcon;
            iconImage.color = Color.gray;
        }

        // Start animations
        StartCoroutine(AnimateNotification(duration));
    }

    private IEnumerator AnimateNotification(float duration)
    {
        // Slide in animation
        float slideInTime = 0.3f;
        Vector3 startPos = transform.localPosition + Vector3.right * 300f;
        Vector3 targetPos = transform.localPosition;

        for (float t = 0; t < slideInTime; t += Time.deltaTime)
        {
            float progress = t / slideInTime;
            transform.localPosition = Vector3.Lerp(startPos, targetPos, Ease(progress));
            yield return null;
        }

        transform.localPosition = targetPos;

        // Wait
        yield return new WaitForSeconds(duration - slideInTime * 2);

        // Fade out animation
        float fadeOutTime = slideInTime;
        float startAlpha = canvasGroup.alpha;

        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            float progress = t / fadeOutTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    private float Ease(float t)
    {
        // Ease out cubic
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
