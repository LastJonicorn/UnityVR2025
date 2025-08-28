using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(NearFarInteractor))]
public class FarTagFilter : MonoBehaviour
{
    public string farTag = "Far";
    private NearFarInteractor interactor;

    void Awake()
    {
        interactor = GetComponent<NearFarInteractor>();
    }

    void Update()
    {
        // Temporarily filter far hits
        if (!interactor.enableFarCasting) return;

        // Check far caster hits
        var farCaster = interactor.farInteractionCaster;
        if (farCaster == null) return;

        var hitColliders = new List<Collider>();
        if (!farCaster.TryGetColliderTargets(interactor.interactionManager, hitColliders)) return;

        bool hasValidFar = false;
        foreach (var c in hitColliders)
        {
            if (c.gameObject.CompareTag(farTag))
            {
                hasValidFar = true;
                break;
            }
        }

        interactor.enableFarCasting = hasValidFar; // Only allow far if a valid object is hit
    }
}
