using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    [Tooltip("Reference to the XR camera (the player's HMD camera).")]
    public Transform xrCamera;

    [Tooltip("How quickly the spectator camera follows the XR camera.")]
    public float smoothTime = 0.1f;

    [Tooltip("Field of view for spectator camera.")]
    public float spectatorFOV = 90f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (xrCamera == null)
        {
            Camera xrCam = Camera.main;
            if (xrCam != null) xrCamera = xrCam.transform;
        }

        GetComponent<Camera>().fieldOfView = spectatorFOV;
    }

    void LateUpdate()
    {
        if (xrCamera == null) return;

        // Smooth position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            xrCamera.position,
            ref velocity,
            smoothTime
        );

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            xrCamera.rotation,
            Time.deltaTime / smoothTime
        );
    }
}
