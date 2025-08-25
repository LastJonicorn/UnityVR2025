using UnityEngine;
using System.Collections;

public class DelayedRigidbodyActivation : MonoBehaviour
{
    public float delayTime = 1.0f; // Adjust this value as needed

    void Start()
    {
        StartCoroutine(ActivateRigidbodies());
    }

    IEnumerator ActivateRigidbodies()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delayTime);

        // Activate all Rigidbody components attached to this GameObject and its children
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false; // Set isKinematic to false to activate Rigidbody physics
        }
    }
}