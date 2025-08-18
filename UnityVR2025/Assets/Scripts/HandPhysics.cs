using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HandPhysics : MonoBehaviour
{
    public Transform Target;
    public bool isRightHand = true;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Position
        rb.linearVelocity = (Target.position - transform.position) / Time.fixedDeltaTime;

        //Rotation
        if (!isRightHand)
        {
            Quaternion postRotation = transform.rotation * Quaternion.Euler(0, 0, -90);
            Quaternion RotationDifference = Target.rotation * Quaternion.Inverse(postRotation);
            RotationDifference.ToAngleAxis(out float AngleInDegree, out Vector3 RotationAxis);
            Vector3 RotationDifferenceInDegree = AngleInDegree * RotationAxis;
            rb.angularVelocity = (RotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
        }
        else
        {
            Quaternion postRotation = transform.rotation * Quaternion.Euler(0, 0, 90);
            Quaternion RotationDifference = Target.rotation * Quaternion.Inverse(postRotation);
            RotationDifference.ToAngleAxis(out float AngleInDegree, out Vector3 RotationAxis);
            Vector3 RotationDifferenceInDegree = AngleInDegree * RotationAxis;
            rb.angularVelocity = (RotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
        }
    }
}
