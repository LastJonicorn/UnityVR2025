using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabHandPose : MonoBehaviour
{
    public float PoseTransitionDuration = 0.2f;
    public HandData RightHandPose;
    public HandData LeftHandPose;

    private Vector3 startHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startHandRotation;
    private Quaternion finalHandRotation;
    private Quaternion[] startFingerRotation;
    private Quaternion[] finalFingerRotation;

    private XRGrabInteractable grabInteractable;
    private float nearDistanceThreshold = 0.2f; // Adjust as needed

    private XRBaseInteractor currentInteractor;
    private bool poseApplied = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);

        RightHandPose.gameObject.SetActive(false);
        LeftHandPose.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (currentInteractor == null) return;

        float distance = Vector3.Distance(currentInteractor.transform.position, grabInteractable.transform.position);

        if (!poseApplied && distance < nearDistanceThreshold)
        {
            // Apply the hand pose
            SetupPose(currentInteractor);
            poseApplied = true;
        }
        else if (poseApplied && distance >= nearDistanceThreshold)
        {
            // Reset the hand pose if object is pulled away
            ResetPose(currentInteractor);
            poseApplied = false;
        }
    }
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
        if (interactor == null) return;

        currentInteractor = interactor;
        poseApplied = false; // Reset so Update can apply pose if near
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
        if (interactor == null) return;

        ResetPose(interactor);
        currentInteractor = null;
        poseApplied = false;
    }

    private void SetupPose(XRBaseInteractor interactor)
    {
        HandData handData = interactor.transform.GetComponentInChildren<HandData>();
        if (handData == null) return;

        handData.Animator.enabled = false;

        if (handData.HandType == HandData.HandModelType.Right)
            SetHandDataValues(handData, RightHandPose);
        else
            SetHandDataValues(handData, LeftHandPose);

        StartCoroutine(SetHandDataRoutine(handData, finalHandPosition, finalHandRotation, finalFingerRotation,
            startHandPosition, startHandRotation, startFingerRotation));
    }

    private void ResetPose(XRBaseInteractor interactor)
    {
        HandData handData = interactor.transform.GetComponentInChildren<HandData>();
        if (handData == null) return;

        handData.Animator.enabled = true;
        StartCoroutine(SetHandDataRoutine(handData, startHandPosition, startHandRotation, startFingerRotation,
            finalHandPosition, finalHandRotation, finalFingerRotation));
    }

    private void SetHandDataValues(HandData h1, HandData h2)
    {
        startHandPosition = h1.Root.localPosition;
        finalHandPosition = h2.Root.localPosition;

        startHandRotation = h1.Root.localRotation;
        finalHandRotation = h2.Root.localRotation;

        startFingerRotation = new Quaternion[h1.FingerBones.Length];
        finalFingerRotation = new Quaternion[h1.FingerBones.Length];

        for (int i = 0; i < h1.FingerBones.Length; i++)
        {
            startFingerRotation[i] = h1.FingerBones[i].localRotation;
            finalFingerRotation[i] = h2.FingerBones[i].localRotation;
        }
    }

    private IEnumerator SetHandDataRoutine(HandData h, Vector3 newPos, Quaternion newRot, Quaternion[] newBoneRot,
        Vector3 startPos, Quaternion startRot, Quaternion[] startBoneRot)
    {
        float timer = 0f;

        while (timer < PoseTransitionDuration)
        {
            float t = timer / PoseTransitionDuration;
            h.Root.localPosition = Vector3.Lerp(startPos, newPos, t);
            h.Root.localRotation = Quaternion.Lerp(startRot, newRot, t);

            for (int i = 0; i < newBoneRot.Length; i++)
            {
                h.FingerBones[i].localRotation = Quaternion.Lerp(startBoneRot[i], newBoneRot[i], t);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure final state
        h.Root.localPosition = newPos;
        h.Root.localRotation = newRot;
        for (int i = 0; i < newBoneRot.Length; i++)
            h.FingerBones[i].localRotation = newBoneRot[i];
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Mirror Selected Right Grab Pose")]
    public static void MirrorRightPose()
    {
        var handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
        handPose.MirrorPose(handPose.LeftHandPose, handPose.RightHandPose);
    }
#endif

    // mirror utility
    public void MirrorPose(HandData poseToMirror, HandData source)
    {
        if (poseToMirror == null || source == null) return;

        Vector3 mirroredPos = source.Root.localPosition; mirroredPos.x *= -1f;

        Quaternion q = source.Root.localRotation;
        Quaternion mirroredRot = new Quaternion(q.x, -q.y, -q.z, q.w);

        poseToMirror.Root.localPosition = mirroredPos;
        poseToMirror.Root.localRotation = mirroredRot;

        for (int i = 0; i < source.FingerBones.Length; i++)
            poseToMirror.FingerBones[i].localRotation = source.FingerBones[i].localRotation;
    }
}
