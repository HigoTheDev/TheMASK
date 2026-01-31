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

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        // Calculate the desired position based on target and offset
        Vector3 desiredPosition = target.position + offset;
        
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

    // Optional: Draw gizmos in the Scene view to visualize the offset
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target.position, target.position + offset);
            Gizmos.DrawWireSphere(target.position + offset, 0.5f);
        }
    }
}
