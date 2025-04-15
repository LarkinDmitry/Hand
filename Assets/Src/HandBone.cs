using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HandBone : MonoBehaviour
{
    [SerializeField] private RealisticGrabHand hand;
    [SerializeField] private Transform bone;
    [SerializeField] private Transform boneEnd;
    [SerializeField] private float boneRadius;
    [Space]
    [SerializeField] private bool switchSize;

    private Transform myTransform;
    private Vector3 boneSize;
    private Vector3 bonePosition;
    private float currentRadius;
    private float currentRatio;
    private float normalRatio = 1.5f;
    private float smallRatio = 2.5f;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        hand.OnSwitchGrabState += SetBoneState;

        currentRadius = boneRadius;
        currentRatio = normalRatio;

        myTransform = transform;
        boneSize = Vector3.zero;
        bonePosition = Vector3.zero;

        meshRenderer = bone.gameObject.GetComponent<MeshRenderer>();
        UIEvents.OnSwitchShowBonesState += SetVisibleState;
        SetVisibleState(UIEvents.ShowBonesState);

        InvokeRepeating(nameof(UpdateBone), 0, 1);
    }

    private void UpdateBone()
    {
        boneSize.x = currentRadius;
        boneSize.y = Vector3.Distance(myTransform.position, boneEnd.position) / currentRatio;
        boneSize.z = currentRadius;

        bonePosition.z = boneSize.y - currentRadius / 2;

        bone.localScale = boneSize;
        bone.localPosition = bonePosition;
    }

    private void SetVisibleState(bool value)
    {
        meshRenderer.enabled = value;
    }

    private void SetBoneState(bool value)
    {
        currentRadius = switchSize && value ? 0.005f : boneRadius;
        currentRatio = switchSize && value ? smallRatio : normalRatio;
        UpdateBone();
    }
}