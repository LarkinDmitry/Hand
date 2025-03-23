using UnityEngine;
using UnityEngine.UI;

public class HandBone : MonoBehaviour
{
    [SerializeField] Transform bone;
    [SerializeField] Transform boneEnd;
    [SerializeField] float boneRadius;

    private Transform myTransform;
    private Vector3 boneSize;
    private Vector3 bonePosition;

    private MeshRenderer meshRenderer;

    private void Start()
    {
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
        boneSize.x = boneRadius;
        boneSize.y = Vector3.Distance(myTransform.position, boneEnd.position) / 1.5f;
        boneSize.z = boneRadius;

        bonePosition.z = boneSize.y - boneRadius / 2;

        bone.localScale = boneSize;
        bone.localPosition = bonePosition;
    }

    private void SetVisibleState(bool value)
    {
        meshRenderer.enabled = value;
    }
}