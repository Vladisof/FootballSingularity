using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Camera Positions")]
    [Tooltip("Позиція камери в головному меню")]
    public Transform menuPosition;
    
    [Tooltip("Позиція камери в грі")]
    public Transform gamePosition;

    [Header("Animation Settings")]
    [Tooltip("Швидкість переміщення камери")]
    public float transitionSpeed = 2f;
    
    [Tooltip("Чи використовувати плавний перехід")]
    public bool useSmoothTransition = true;

    private Transform targetPosition;
    private bool isTransitioning = false;

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

    private void Start()
    {
        // За замовчуванням камера в позиції меню
        if (menuPosition != null)
        {
            transform.position = menuPosition.position;
            transform.rotation = menuPosition.rotation;
        }
    }

    private void Update()
    {
        if (isTransitioning && targetPosition != null)
        {
            // Плавне переміщення камери до цільової позиції
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetPosition.rotation, Time.deltaTime * transitionSpeed);

            // Перевірити, чи досягли ми цільової позиції
            float distance = Vector3.Distance(transform.position, targetPosition.position);
            float angleDistance = Quaternion.Angle(transform.rotation, targetPosition.rotation);
            
            if (distance < 0.01f && angleDistance < 0.5f)
            {
                // Зафіксувати точну позицію
                transform.position = targetPosition.position;
                transform.rotation = targetPosition.rotation;
                isTransitioning = false;
                
                Debug.Log($"Camera transition complete to {targetPosition.name}");
            }
        }
    }

    /// <summary>
    /// Переміщує камеру в позицію головного меню
    /// </summary>
    public void MoveToMenuPosition()
    {
        if (menuPosition == null)
        {
            Debug.LogWarning("Menu position is not set!");
            return;
        }

        Debug.Log($"MoveToMenuPosition called. Current position: {transform.position}, Target: {menuPosition.position}");

        if (useSmoothTransition)
        {
            targetPosition = menuPosition;
            isTransitioning = true;
            Debug.Log("Moving camera to menu position (smooth)");
        }
        else
        {
            transform.position = menuPosition.position;
            transform.rotation = menuPosition.rotation;
            Debug.Log("Snapped camera to menu position");
        }
    }

    /// <summary>
    /// Переміщує камеру в позицію гри
    /// </summary>
    public void MoveToGamePosition()
    {
        if (gamePosition == null)
        {
            Debug.LogWarning("Game position is not set!");
            return;
        }

        Debug.Log($"MoveToGamePosition called. Current position: {transform.position}, Target: {gamePosition.position}");

        if (useSmoothTransition)
        {
            targetPosition = gamePosition;
            isTransitioning = true;
            Debug.Log("Moving camera to game position (smooth)");
        }
        else
        {
            transform.position = gamePosition.position;
            transform.rotation = gamePosition.rotation;
            Debug.Log("Snapped camera to game position");
        }
    }

    /// <summary>
    /// Миттєво переміщує камеру в позицію меню без анімації
    /// </summary>
    public void SnapToMenuPosition()
    {
        if (menuPosition != null)
        {
            transform.position = menuPosition.position;
            transform.rotation = menuPosition.rotation;
            isTransitioning = false;
        }
    }

    /// <summary>
    /// Миттєво переміщує камеру в позицію гри без анімації
    /// </summary>
    public void SnapToGamePosition()
    {
        if (gamePosition != null)
        {
            transform.position = gamePosition.position;
            transform.rotation = gamePosition.rotation;
            isTransitioning = false;
        }
    }

    /// <summary>
    /// Перевіряє, чи відбувається перехід камери
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
}
