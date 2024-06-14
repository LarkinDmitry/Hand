using UnityEngine;

public class Gripper : MonoBehaviour
{
    [SerializeField] private TriggerZone grabZone;
    [SerializeField] private TriggerZone tooMatchZone;
    
    public HandPart HandPart { get; private set; }
    public Hand Hand { get; private set; }

    private GrableObj grableObj;
    private Material material;
    private Color green = new(0, 1, 0, 0.2f);
    private Color red = new(1, 0, 0, 0.2f);
    private Color white = new(1, 1, 1, 0.2f);

    private void Start()
    {
        grabZone.OnChangeStatus += CheckStatus;
        tooMatchZone.OnChangeStatus += CheckStatus;
        material = grabZone.gameObject.GetComponent<MeshRenderer>().material;
    }

    public void Iinitialization(HandPart handPart, Hand hand)
    {
        HandPart = handPart;
        Hand = hand;
    }

    private void CheckStatus()
    {
        if (grabZone.CollisionObjt != null && tooMatchZone.CollisionObjt == null)
        {
            grableObj = grabZone.CollisionObjt;
            grableObj.AddGripper(this);
        }
        else if (grableObj != null)
        {
            grableObj.RemoveGripper(this);
            grableObj = null;
        }

        Updatematerial();
    }

    private void Updatematerial()
    {
        if (grabZone.CollisionObjt != null && tooMatchZone.CollisionObjt == null)
        {
            material.color = green;
        }
        else if (tooMatchZone.CollisionObjt != null)
        {
            material.color = red;
        }
        else
        {
            material.color = white;
        }
    }
}