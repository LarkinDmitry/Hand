using UnityEngine;

public class Gripper : MonoBehaviour
{
    [SerializeField] private TriggerZone grabZone;
    
    public HandPart HandPart { get; private set; }
    public Hand Hand { get; private set; }

    private GrableObj grableObj;
    private Material material;
    private Color green = new(0, 1, 0, 0.2f);
    private Color white = new(1, 1, 1, 0.2f);

    private MeshRenderer meshRenderer;

    private void Start()
    {
        grabZone.OnChangeStatus += CheckStatus;
        material = grabZone.gameObject.GetComponent<MeshRenderer>().material;
        meshRenderer = grabZone.gameObject.GetComponent<MeshRenderer>();

        UIEvents.OnSwitchShowGrabZoneState += SetVisibleState;
        SetVisibleState(UIEvents.ShowGrabZoneState);
    }

    public void Iinitialization(HandPart handPart, Hand hand)
    {
        HandPart = handPart;
        Hand = hand;
    }

    private void CheckStatus()
    {
        if (grabZone.CollisionObjt != null)
        {
            grableObj = grabZone.CollisionObjt;
            grableObj.AddGripper(this);
        }
        else if (grableObj != null)
        {
            grableObj.RemoveGripper(this);
            grableObj = null;
        }

        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (grabZone.CollisionObjt != null)
        {
            material.color = green;
        }
        else
        {
            material.color = white;
        }
    }

    private void SetVisibleState(bool value)
    {
        meshRenderer.enabled = value;
    }
}