using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandColliderManager : MonoBehaviour
{
    [Tooltip("Interactor that fires grab/release events (Near-Far/Direct). If left empty, will search in parents.")]
    public XRBaseInteractor interactor;

    [Tooltip("Root of the hand model that contains the physical colliders (palm/fingers).")]
    public Transform handRoot;

    [Tooltip("Delay before re-enabling colliders after release to avoid physics pops.")]
    public float reenableDelay = 0.05f;

    private readonly List<Collider> _handColliders = new List<Collider>();

    void Reset()
    {
        // Helpful defaults when adding the component
        interactor = GetComponentInParent<XRBaseInteractor>();
        handRoot = transform;
    }

    void Awake()
    {
        if (interactor == null)
            interactor = GetComponentInParent<XRBaseInteractor>();

        if (interactor == null)
        {
            Debug.LogError($"{name}: No XRBaseInteractor found. Assign one in the inspector.");
            enabled = false;
            return;
        }

        if (handRoot == null)
            handRoot = transform;

        _handColliders.Clear();
        foreach (var c in handRoot.GetComponentsInChildren<Collider>(true))
        {
            // Only manage solid colliders; keep triggers (poke sensors, etc.) untouched
            if (!c.isTrigger)
                _handColliders.Add(c);
        }

        // Subscribe with parameterless lambdas to avoid signature/version issues
        interactor.selectEntered.AddListener(_ => OnGrab());
        interactor.selectExited.AddListener(_ => OnRelease());
    }

    // Note: removing specific lambda listeners is messy; usually fine since the hand lives with the interactor.
    // If you need strict cleanup, store the delegates and use matching types for your XRI version.

    private void OnGrab()
    {
        SetHandColliders(false);
    }

    private void OnRelease()
    {
        CancelInvoke(nameof(ReenableColliders));
        Invoke(nameof(ReenableColliders), reenableDelay);
    }

    private void ReenableColliders()
    {
        SetHandColliders(true);
    }

    private void SetHandColliders(bool enabled)
    {
        foreach (var c in _handColliders)
            c.enabled = enabled;
    }
}
