using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandColliderManager : MonoBehaviour
{
    private XRDirectInteractor interactor;
    private List<Collider> handColliders = new List<Collider>();

    [Tooltip("Delay before re-enabling colliders after release, to avoid collision flicks.")]
    public float reenableDelay = 0.05f;

    void Awake()
    {
        interactor = GetComponent<XRDirectInteractor>();

        // Gather all colliders in the hand (but skip triggers if you want to keep sensors like finger tip triggers).
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            if (!col.isTrigger)
                handColliders.Add(col);
        }

        // Hook into grab events
        interactor.selectEntered.AddListener(OnGrab);
        interactor.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        // Clean up listeners
        interactor.selectEntered.RemoveListener(OnGrab);
        interactor.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        SetHandCollidersActive(false);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Re-enable after small delay to avoid physics explosion
        Invoke(nameof(ReenableColliders), reenableDelay);
    }

    private void ReenableColliders()
    {
        SetHandCollidersActive(true);
    }

    private void SetHandCollidersActive(bool active)
    {
        foreach (var col in handColliders)
        {
            col.enabled = active;
        }
    }
}
