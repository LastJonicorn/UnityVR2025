using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrabHandPose : MonoBehaviour
{
    public float PoseTransitionDuration = 0.2f;
    public HandData RightHandPose;
    public HandData LeftHandPose;
    private Vector3 StartHandPosition;
    private Vector3 FinalHandPosition;
    private Quaternion StartHandRotation;
    private Quaternion FinalHandRotation;

    private Quaternion[] StartFingerRotation;
    private Quaternion[] FinalFingerRotation;

    private int originalLayer;


    // I don't fucking know
    void Start()
    {
        originalLayer = gameObject.layer;

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);

        RightHandPose.gameObject.SetActive(false);
        LeftHandPose.gameObject.SetActive(false);
    }

    //Set custom hand position
    public void SetupPose(BaseInteractionEventArgs arg)
    {
        gameObject.layer = LayerMask.NameToLayer("Grabbed");

        if (arg.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.Animator.enabled = false;

            if (handData.HandType == HandData.HandModelType.Right) 
            {
                SetHandDataValues(handData, RightHandPose);
            }
            else
            {
                SetHandDataValues(handData, LeftHandPose);
            }

            StartCoroutine(SetHandDataRoutine(handData, FinalHandPosition, FinalHandRotation, FinalFingerRotation, StartHandPosition, StartHandRotation, StartFingerRotation));
        }
    }

    //Set hand position back to default after letting go of object
    public void UnSetPose(BaseInteractionEventArgs arg)
    {
        gameObject.layer = originalLayer;

        if (arg.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.Animator.enabled = true;

            if (handData.HandType == HandData.HandModelType.Right)
            {
                StartCoroutine(SetHandDataRoutine(handData, StartHandPosition, StartHandRotation, StartFingerRotation, FinalHandPosition, FinalHandRotation, FinalFingerRotation));
            }
            else
            {
                StartCoroutine(SetHandDataRoutine(handData, StartHandPosition, StartHandRotation, StartFingerRotation, FinalHandPosition, FinalHandRotation, FinalFingerRotation));
            }
        }
    }


    public void SetHandDataValues(HandData h1, HandData h2)
    {
        StartHandPosition = new Vector3
            (
                h1.Root.localPosition.x / h1.Root.localScale.x, 
                h1.Root.localPosition.y / h1.Root.localScale.y, 
                h1.Root.localPosition.z / h1.Root.localScale.z
            );
        FinalHandPosition = new Vector3
            (
                h2.Root.localPosition.x / h2.Root.localScale.x, 
                h2.Root.localPosition.y / h2.Root.localScale.y, 
                h2.Root.localPosition.z / h2.Root.localScale.z
            );

        StartHandRotation = h1.Root.localRotation;
        FinalHandRotation = h2.Root.localRotation;

        StartFingerRotation = new Quaternion[h1.FingerBones.Length];
        FinalFingerRotation = new Quaternion[h1.FingerBones.Length];

        for(int i = 0; i < h1.FingerBones.Length; i++)
        {
            StartFingerRotation[i] = h1.FingerBones[i].localRotation;
            FinalFingerRotation[i] = h2.FingerBones[i].localRotation;
        }
    }

    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBoneRotation)
    {
        h.Root.localPosition = newPosition;
        h.Root.localRotation = newRotation;

        for(int i = 0; i < newBoneRotation.Length; i++)
        {
            h.FingerBones[i].localRotation = newBoneRotation[i];
        }
    }

    // Smooth transition from pose to pose
    public IEnumerator SetHandDataRoutine(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBoneRotation, Vector3 startingPosition, Quaternion startingRotation, Quaternion[] StartingBoneRotation)
    {
        float timer = 0;

        while (timer < PoseTransitionDuration)
        {
            Vector3 p = Vector3.Lerp(startingPosition, newPosition, timer / PoseTransitionDuration);
            Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / PoseTransitionDuration);

            h.Root.localPosition = p;
            h.Root.localRotation = r;

            for (int i = 0; i < newBoneRotation.Length; i++)
            {
                h.FingerBones[i].localRotation = Quaternion.Lerp(StartingBoneRotation[i], newBoneRotation[i], timer / PoseTransitionDuration);
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Mirror Selected Right Grab Pose")]
    public static void MirrorRightPose()
    {
        GrabHandPose handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
        handPose.MirrorPose(handPose.LeftHandPose, handPose.RightHandPose);
    }

#endif
    // Mirror the pose set to right hand to left hand
    public void MirrorPose(HandData poseToMirror, HandData poseUsedToMirror)
    {
        Vector3 mirroredPosition = poseUsedToMirror.Root.localPosition;
        mirroredPosition.x *= -1;

        Quaternion mirroredQuaternion = poseUsedToMirror.Root.localRotation;
        mirroredQuaternion.y *= -1;
        mirroredQuaternion.z *= -1;

        poseToMirror.Root.localPosition = mirroredPosition;
        poseToMirror.Root.localRotation = mirroredQuaternion;

        for (int i = 0; i < poseUsedToMirror.FingerBones.Length; i++)
        {
            poseToMirror.FingerBones[i].localRotation = poseUsedToMirror.FingerBones[i].localRotation;
        }
    }
}
