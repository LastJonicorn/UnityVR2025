using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysics : MonoBehaviour
{
    public Transform Target;
    public bool isRightHand = true;
    private Rigidbody rb;

    [HideInInspector] public bool useCustomPose = false; // Used in GrabHandPose

    // Offset to bring hand closer/further from controller
    [SerializeField] private Vector3 localPositionOffset = new Vector3(0f, 0f, -0.05f);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Skip physics if we're using a custom pose
        if (useCustomPose)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;

            // Apply offset relative to Target's rotation
            Vector3 targetPosition = Target.position + Target.rotation * localPositionOffset;

            // Position
            rb.linearVelocity = (targetPosition - transform.position) / Time.fixedDeltaTime;

            // Rotation
            Quaternion postRotation = transform.rotation * Quaternion.Euler(0, 0, isRightHand ? 90 : -90);
            Quaternion rotationDiff = Target.rotation * Quaternion.Inverse(postRotation);
            rotationDiff.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);
            Vector3 rotationDifference = angleInDegree * rotationAxis;
            rb.angularVelocity = (rotationDifference * Mathf.Deg2Rad / Time.fixedDeltaTime);
        }
    }
}
