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

    private MeshRenderer meshRenderer;
    private bool vibration = false;

    private void Start()
    {
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
            VibrationFeedbackController.Instance.SetVibrationPercent(Hand.Side, HandPart, 0);
            vibration = false;
        }
    }

    private IEnumerator MakeVibro()
    {
        if (!vibration)
        {
            vibration = true;
            VibrationFeedbackController.Instance.SetVibrationPercent(Hand.Side, HandPart, 50);
            yield return new WaitForSeconds(0.025f);
            VibrationFeedbackController.Instance.SetVibrationPercent(Hand.Side, HandPart, 5);
        }
    }
}