using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandleGhostHand : MonoBehaviour
{
    [Header("Custom Hand Prefabs inside Handle")]
    public HandData RightHandPose;
    public HandData LeftHandPose;

    private HandData activeHand;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        if (RightHandPose != null) RightHandPose.gameObject.SetActive(false);
        if (LeftHandPose != null) LeftHandPose.gameObject.SetActive(false);

        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        activeHand = args.interactorObject.transform.GetComponentInChildren<HandData>();
        if (activeHand == null) return;

        DisableHandRenderers(activeHand);

        HandData ghost = activeHand.HandType == HandData.HandModelType.Right ? RightHandPose : LeftHandPose;
        if (ghost != null)
            ghost.gameObject.SetActive(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        HandData hand = args.interactorObject.transform.GetComponentInChildren<HandData>();
        if (hand == null) return;

        EnableHandRenderers(hand);

        HandData ghost = hand.HandType == HandData.HandModelType.Right ? RightHandPose : LeftHandPose;
        if (ghost != null)
            ghost.gameObject.SetActive(false);

        if (hand == activeHand)
            activeHand = null;
    }

    private void DisableHandRenderers(HandData hand)
    {
        if (hand.Animator != null) hand.Animator.enabled = false;
        foreach (var r in hand.GetComponentsInChildren<Renderer>())
            r.enabled = false;
    }

    private void EnableHandRenderers(HandData hand)
    {
        if (hand.Animator != null) hand.Animator.enabled = true;
        foreach (var r in hand.GetComponentsInChildren<Renderer>())
            r.enabled = true;
    }
}
