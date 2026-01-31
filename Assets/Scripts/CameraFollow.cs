using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The player character to follow")]
    [SerializeField] private Transform target;
    
    [Tooltip("Background transform to move along with camera (optional)")]
    [SerializeField] private Transform background;
    
    [Header("Follow Settings")]
    [Tooltip("Offset from the target position")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    
    [Tooltip("Speed of the camera smoothing (0.1 = slow, 0.5 = fast)")]
    [SerializeField] private float smoothSpeed = 0.125f;
    
    [Tooltip("Enable smooth following or snap instantly to target")]
    [SerializeField] private bool useSmoothing = true;
    
    [Tooltip("Follow target on X axis")]
    [SerializeField] private bool followX = true;
    
    [Tooltip("Follow target on Y axis")]
    [SerializeField] private bool followY = true;
    
    [Tooltip("Follow target on Z axis")]
    [SerializeField] private bool followZ = true;
    
    [Header("Position Limits")]
    [Tooltip("Enable position limits to restrict camera movement")]
    [SerializeField] private bool useLimits = false;
    
    [Tooltip("Minimum position bounds (X, Y, Z)")]
    [SerializeField] private Vector3 minPosition = new Vector3(-10, 0, -10);
    
    [Tooltip("Maximum position bounds (X, Y, Z)")]
    [SerializeField] private Vector3 maxPosition = new Vector3(10, 10, 10);
    
    [Header("Background Settings")]
    [Tooltip("Parallax speed for background (0 = no movement, 1 = same as camera, 0.5 = half speed)")]
    [SerializeField] private float backgroundParallaxSpeed = 1.0f;
    
    [Tooltip("Move background on X axis")]
    [SerializeField] private bool backgroundFollowX = true;
    
    [Tooltip("Move background on Y axis")]
    [SerializeField] private bool backgroundFollowY = false;
    
    private Vector3 _backgroundStartPosition;
    private Vector3 _cameraStartPosition;

    void Start()
    {
        // Store starting positions
        _cameraStartPosition = transform.position;
        if (background != null)
        {
            _backgroundStartPosition = background.position;
        }
    }
    
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        // Calculate the desired position based on target and offset
        Vector3 desiredPosition = target.position + offset;
        
        // Apply axis locks (keep current position for locked axes)
        if (!followX) desiredPosition.x = transform.position.x;
        if (!followY) desiredPosition.y = transform.position.y;
        if (!followZ) desiredPosition.z = transform.position.z;
        
        // Apply limits if enabled
        if (useLimits)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosition.x, maxPosition.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPosition.y, maxPosition.y);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minPosition.z, maxPosition.z);
        }
        
        if (useSmoothing)
        {
            // Smoothly interpolate between current and desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else
        {
            // Instantly move to desired position
            transform.position = desiredPosition;
        }
        
        // Move background if assigned
        if (background != null)
        {
            UpdateBackgroundPosition();
        }
    }
    
    private void UpdateBackgroundPosition()
    {
        // Calculate how much the camera has moved from start
        Vector3 cameraMovement = transform.position - _cameraStartPosition;
        
        // Apply parallax effect
        Vector3 parallaxMovement = cameraMovement * backgroundParallaxSpeed;
        
        // Apply axis locks
        if (!backgroundFollowX) parallaxMovement.x = 0;
        if (!backgroundFollowY) parallaxMovement.y = 0;
        parallaxMovement.z = 0; // Always lock Z for 2D backgrounds
        
        // Set background position
        background.position = _backgroundStartPosition + parallaxMovement;
    }

    // Optional: Draw gizmos in the Scene view to visualize the offset and limits
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target.position, target.position + offset);
            Gizmos.DrawWireSphere(target.position + offset, 0.5f);
        }
        
        // Draw the limit bounds
        if (useLimits)
        {
            Gizmos.color = Color.red;
            Vector3 center = (minPosition + maxPosition) / 2;
            Vector3 size = maxPosition - minPosition;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
