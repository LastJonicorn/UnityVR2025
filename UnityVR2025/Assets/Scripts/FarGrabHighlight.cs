using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FarGrabHighlight : MonoBehaviour
{
    [Tooltip("Prefab of the circle highlight to spawn next to objects.")]
    public GameObject highlightPrefab;

    private XRBaseInteractor[] interactors;
    private GameObject activeHighlight;
    private SpriteRenderer activeRenderer;
    private Coroutine fadeRoutine;

    private Transform currentHoveredObject;

    void Awake()
    {
        // Grab all interactors (Sphere + Curve + NearFar) on this object
        interactors = GetComponents<XRBaseInteractor>();

        if (interactors == null || interactors.Length == 0)
        {
            Debug.LogError($"{nameof(FarGrabHighlight)}: No XRBaseInteractor components found on {name}");
            enabled = false;
            return;
        }

        foreach (var interactor in interactors)
        {
            interactor.hoverEntered.AddListener(OnHoverEntered);
            interactor.hoverExited.AddListener(OnHoverExited);
            interactor.selectEntered.AddListener(OnGrab);
        }
    }

    void Update()
    {
        if (activeHighlight != null && currentHoveredObject != null)
        {
            // Update position every frame to match the hovered object
            Collider col = currentHoveredObject.GetComponentInChildren<Collider>();
            Vector3 center = col != null ? col.bounds.center : currentHoveredObject.position;
            activeHighlight.transform.position = center;

            // Keep facing the camera
            activeHighlight.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }


    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Transform hovered = args.interactableObject.transform;

        // Only show highlight if object layer is "FarInteract"
        if (hovered.gameObject.layer != LayerMask.NameToLayer("FarInteract"))
            return;

        // Clear previous highlight immediately if it exists
        ClearHighlight();

        currentHoveredObject = hovered;

        // Find center of the object
        Collider col = hovered.GetComponentInChildren<Collider>();
        Vector3 center = col != null ? col.bounds.center : hovered.position;

        // Instantiate highlight at center
        activeHighlight = Instantiate(highlightPrefab);
        activeHighlight.transform.position = center;

        // Face camera
        activeHighlight.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        activeRenderer = activeHighlight.GetComponent<SpriteRenderer>();
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        // Only clear highlight if it's the object we just stopped hovering
        if (currentHoveredObject == args.interactableObject.transform)
        {
            currentHoveredObject = null;
            ClearHighlight();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        ClearHighlight();
    }

    private void ClearHighlight()
    {
        if (activeHighlight != null)
        {
            Destroy(activeHighlight);
            activeHighlight = null;
            activeRenderer = null;
        }
    }
}
