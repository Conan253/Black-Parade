using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;   // Drag your "Gerard" player here in Inspector
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [SerializeField] private float pixelsPerUnit = 100f; // Match your sprites’ PPU

    void LateUpdate()
    {
        if (player == null) return;

        // Desired camera position
        Vector3 desiredPosition = player.position + offset;

        // Smoothly move camera (optional, can remove if you want instant follow)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Pixel-perfect rounding
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * pixelsPerUnit) / pixelsPerUnit;
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * pixelsPerUnit) / pixelsPerUnit;

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
