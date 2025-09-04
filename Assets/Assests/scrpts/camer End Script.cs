using UnityEngine;

public class StopCameraFollow : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
            if (camFollow != null)
            {
                camFollow.enabled = false;   // disables the camera follow script
            }
        }
    }
}
