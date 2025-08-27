using System.Collections;
using UnityEngine;

public class WallHandPose : MonoBehaviour
{
    public HandData LeftHandPose;
    public HandData RightHandPose;
    public float PoseTransitionDuration = 0.2f;

    private HandData activeHand;
    private Quaternion[] startFingerRotation;
    private Quaternion[] finalFingerRotation;
    private Quaternion[] originalFingerRotation; // stores default before entering

    void Start()
    {
        if (LeftHandPose != null) LeftHandPose.gameObject.SetActive(false);
        if (RightHandPose != null) RightHandPose.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        HandData hand = other.GetComponentInChildren<HandData>();
        if (hand == null) return;

        activeHand = hand;
        hand.Animator.enabled = false;

        HandData pose = hand.HandType == HandData.HandModelType.Right ? RightHandPose : LeftHandPose;
        if (pose == null) return;

        startFingerRotation = new Quaternion[hand.FingerBones.Length];
        finalFingerRotation = new Quaternion[hand.FingerBones.Length];
        originalFingerRotation = new Quaternion[hand.FingerBones.Length];

        for (int i = 0; i < hand.FingerBones.Length; i++)
        {
            if (hand.FingerBones[i] == null || pose.FingerBones[i] == null) continue;

            // Save current rotation as start
            startFingerRotation[i] = hand.FingerBones[i].localRotation;
            originalFingerRotation[i] = hand.FingerBones[i].localRotation;

            // Target rotation
            finalFingerRotation[i] = pose.FingerBones[i].localRotation;
        }

        StartCoroutine(BlendFingers(hand, startFingerRotation, finalFingerRotation));
    }

    void OnTriggerExit(Collider other)
    {
        HandData hand = other.GetComponentInChildren<HandData>();
        if (hand == null || hand != activeHand) return;

        StartCoroutine(BlendFingers(hand, finalFingerRotation, originalFingerRotation, () =>
        {
            hand.Animator.enabled = true;
            activeHand = null;
        }));
    }

    private IEnumerator BlendFingers(HandData hand, Quaternion[] start, Quaternion[] end, System.Action onComplete = null)
    {
        float timer = 0f;
        while (timer < PoseTransitionDuration)
        {
            float t = timer / PoseTransitionDuration;
            for (int i = 0; i < hand.FingerBones.Length; i++)
            {
                if (hand.FingerBones[i] != null)
                    hand.FingerBones[i].localRotation = Quaternion.Slerp(start[i], end[i], t);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < hand.FingerBones.Length; i++)
        {
            if (hand.FingerBones[i] != null)
                hand.FingerBones[i].localRotation = end[i];
        }

        onComplete?.Invoke();
    }
}
