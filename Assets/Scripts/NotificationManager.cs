using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("Notification Settings")]
    public GameObject notificationPrefab;
    public Transform notificationContainer;
    public float notificationDuration = 3f;
    public int maxNotifications = 5;

    private Queue notificationQueue = new Queue();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowNotification(string message, NotificationType type = NotificationType.Info)
    {
        if (notificationPrefab == null || notificationContainer == null)
        {
            Debug.LogWarning($"Notification: {message}");
            return;
        }

        // Create notification
        GameObject notification = Instantiate(notificationPrefab, notificationContainer);
        NotificationUI notificationUI = notification.GetComponent<NotificationUI>();

        if (notificationUI != null)
        {
            notificationUI.Setup(message, type, notificationDuration);
        }

        // Play sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayNotification();
        }

        // Add to queue and manage count
        notificationQueue.Enqueue(notification);
        
        while (notificationQueue.Count > maxNotifications)
        {
            GameObject oldNotification = notificationQueue.Dequeue() as GameObject;
            if (oldNotification != null)
            {
                Destroy(oldNotification);
            }
        }

        // Auto destroy after duration
        StartCoroutine(DestroyAfterDelay(notification, notificationDuration));
    }

    private IEnumerator DestroyAfterDelay(GameObject notification, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (notification != null)
        {
            Destroy(notification);
        }
    }

    // Convenience methods
    public void ShowSuccess(string message)
    {
        ShowNotification(message, NotificationType.Success);
    }

    public void ShowError(string message)
    {
        ShowNotification(message, NotificationType.Error);
    }

    public void ShowWarning(string message)
    {
        ShowNotification(message, NotificationType.Warning);
    }

    public void ShowInfo(string message)
    {
        ShowNotification(message, NotificationType.Info);
    }
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}

