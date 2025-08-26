using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KinematicOnGrab : MonoBehaviour
{
    public Rigidbody targetRigidbody;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError($"[KinematicOnGrab] No XRGrabInteractable found on {gameObject.name}!");
            enabled = false;
            return;
        }

        if (targetRigidbody == null)
            targetRigidbody = GetComponent<Rigidbody>();

        if (targetRigidbody == null)
        {
            Debug.LogError($"[KinematicOnGrab] No Rigidbody found on {gameObject.name}!");
            enabled = false;
            return;
        }

        targetRigidbody.isKinematic = true;

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args) => targetRigidbody.isKinematic = false;
    private void OnReleased(SelectExitEventArgs args) => targetRigidbody.isKinematic = true;

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}
