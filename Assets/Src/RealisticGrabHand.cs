using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RealisticGrabHand : MonoBehaviour
{
    [SerializeField] private HandBones wristOriginal;
    [SerializeField] private SkinnedMeshRenderer handOriginal;
    [Space]
    [SerializeField] private HandBones wristView;
    [SerializeField] private SkinnedMeshRenderer handView;
    [Space]
    [SerializeField] private Gripper fingerGrabZonePrefab;
    [Space]
    [SerializeField] private Gripper palmGrabZonePrefab;
    [Space]
    [SerializeField] private float threshold;
    [Space]
    [SerializeField] private Toggle filtration;

    private Hand hand;

    private void Awake()
    {
        Gripper palm = Instantiate(palmGrabZonePrefab, wristView.palm.transform);
        Gripper thumb = Instantiate(fingerGrabZonePrefab, wristView.thumbTip.transform);
        Gripper index = Instantiate(fingerGrabZonePrefab, wristView.indexTip.transform);
        Gripper middle = Instantiate(fingerGrabZonePrefab, wristView.middleTip.transform);
        Gripper ring = Instantiate(fingerGrabZonePrefab, wristView.ringTip.transform);
        Gripper little = Instantiate(fingerGrabZonePrefab, wristView.littleTip.transform);

        hand = new (palm, thumb, index, middle, ring, little);
    }

    private void Update()
    {
        handView.enabled = handOriginal.enabled;

        if (handView.enabled)
        {
            wristView.wrist.localPosition = wristOriginal.wrist.localPosition;
            wristView.wrist.localRotation = wristOriginal.wrist.localRotation;
            wristView.palm.localPosition = wristOriginal.palm.localPosition;
            wristView.palm.localRotation = wristOriginal.palm.localRotation;

            UpdateBonePosition(HandPart.Thumb, wristView, wristOriginal);
            UpdateBonePosition(HandPart.Index, wristView, wristOriginal);
            UpdateBonePosition(HandPart.Middle, wristView, wristOriginal);
            UpdateBonePosition(HandPart.Ring, wristView, wristOriginal);
            UpdateBonePosition(HandPart.Pinkie, wristView, wristOriginal);
        }
    }

    private void UpdateBonePosition(HandPart handPart, HandBones view, HandBones original)
    {
        if (hand.GripParts != null && hand.GripParts.Contains(handPart) && filtration.isOn)
        {
            Vector3 viewPosition = view.GetGroupByPart(handPart)[0].position - view.palm.position;
            Vector3 originalPosition = original.GetGroupByPart(handPart)[0].position - original.palm.position;

            if (Vector3.Distance(viewPosition, originalPosition) > threshold)
            {
                for (int i = 0; i < view.GetGroupByPart(handPart).Length; i++)
                {
                    CopyPositionAndRotation(view.GetGroupByPart(handPart)[i], original.GetGroupByPart(handPart)[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < view.GetGroupByPart(handPart).Length; i++)
            {
                CopyPositionAndRotation(view.GetGroupByPart(handPart)[i], original.GetGroupByPart(handPart)[i]);
            }
        }

        void CopyPositionAndRotation(Transform view, Transform original)
        {
            view.SetLocalPositionAndRotation(original.localPosition, original.localRotation);
        }
    }
}