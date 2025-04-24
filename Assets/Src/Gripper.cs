using System.Collections;
using UnityEngine;

public class Gripper : MonoBehaviour
{
    [SerializeField] private TriggerZone grabZone;
    
    public HandPart HandPart { get; private set; }
    public Hand Hand { get; private set; }

    private Material material;
    private Color green = new(0, 1, 0, 0.2f);
    private Color white = new(1, 1, 1, 0.2f);

    private float vibrationTrashHold;

    private MeshRenderer meshRenderer;
    private VibrationFeedbackController vibration;

    private void Start()
    {
        vibration = VibrationFeedbackController.Instance;

        grabZone.OnAddObj += (obj) =>
        {
            //Hand.AddCollisionObj(obj);
            obj.AddGripper(this);
            material.color = green;
            SetVibroFeedback(true);
        };

        grabZone.OnDelObj += (obj) =>
        {
            //Hand.RemoveCollisionObj(obj);
            obj.RemoveGripper(this);
            material.color = white;
            SetVibroFeedback(false);
        };

        material = grabZone.gameObject.GetComponent<MeshRenderer>().material;
        meshRenderer = grabZone.gameObject.GetComponent<MeshRenderer>();

        UIEvents.OnSwitchShowGrabZoneState += SetVisibleState;
        SetVisibleState(UIEvents.ShowGrabZoneState);
    }

    private void Update()
    {
        if (vibrationTrashHold > 0)
        {
            vibrationTrashHold = Mathf.Clamp(vibrationTrashHold -= Time.deltaTime, 0, float.MaxValue);
        }
    }

    public void Iinitialization(HandPart handPart, Hand hand)
    {
        HandPart = handPart;
        Hand = hand;
    }

    private void SetVisibleState(bool value)
    {
        meshRenderer.enabled = value;
    }

    private void SetVibroFeedback(bool state)
    {
        if (state)
        {
            StartCoroutine(MakeVibro());
        }
        else
        {
            StopAllCoroutines();
            vibrationTrashHold = vibration.impactTrashhold;
            vibration.SetVibrationPercent(Hand.Side, HandPart, 0);
        }
    }

    private IEnumerator MakeVibro()
    {
        if(vibrationTrashHold <= 0)
        {
            vibration.SetVibrationPercent(Hand.Side, HandPart, vibration.impactForce);
            yield return new WaitForSeconds(vibration.impactDuration);
        }

        VibrationFeedbackController.Instance.SetVibrationPercent(Hand.Side, HandPart, vibration.holdingForce);
    }
}