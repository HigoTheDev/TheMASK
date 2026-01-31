using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The player character to follow")]
    [SerializeField] private Transform target;
    
    [Header("Follow Settings")]
    [Tooltip("Offset from the target position")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    
    [Tooltip("Speed of the camera smoothing (0.1 = slow, 0.5 = fast)")]
    [SerializeField] private float smoothSpeed = 0.125f;
    
    [Tooltip("Enable smooth following or snap instantly to target")]
    [SerializeField] private bool useSmoothing = true;
    
    [Header("Position Limits")]
    [Tooltip("Enable position limits to restrict camera movement")]
    [SerializeField] private bool useLimits = false;
    
    [Tooltip("Minimum position bounds (X, Y, Z)")]
    [SerializeField] private Vector3 minPosition = new Vector3(-10, 0, -10);
    
    [Tooltip("Maximum position bounds (X, Y, Z)")]
    [SerializeField] private Vector3 maxPosition = new Vector3(10, 10, 10);

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        // Calculate the desired position based on target and offset
        Vector3 desiredPosition = target.position + offset;
        
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
