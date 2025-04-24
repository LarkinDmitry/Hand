using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrableObj : MonoBehaviour
{
    [SerializeField, Header("Обязательные части руки (без контакта с этими частями захвата не будет)")]
    private HandPart[] requiredGrabHandParts;
    [SerializeField, Header("Вспомогательные части руки (допускается контакт не со всеми частями)")]
    private HandPart[] optionalGrabHandParts;
    [SerializeField, Header("Сколько второстепенных частей допутимо пропустить")]
    private int maxSkipParts;

    private Transform defaultParent;
    private List<Gripper> grippers = new();
    private Rigidbody rb;

    private Vector3 startPosition;
    private Quaternion startRotation;
    public Hand GripHand { get; private set; }

    private void Awake()
    {
        defaultParent = transform.parent;
        rb = GetComponent<Rigidbody>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        UIEvents.OnPressReset += GoToStartPosition;
    }

    public void AddGripper(Gripper gripper)
    {
        grippers.Add(gripper);
        CheckStatus();
    }

    public void RemoveGripper(Gripper gripper)
    {
        grippers.Remove(gripper);
        CheckStatus();
    }

    public bool IsGrippedAt(Hand hand)
    {
        foreach (var gripper in grippers)
        {
            if(gripper.Hand == hand)
            {
                return true;
            }
        }

        return false;
    }

    private void CheckStatus()
    {
        int requiredSkips = 0;
        for (int i = 0; i < requiredGrabHandParts.Length; i++)
        {
            bool requiredResult = false;
            foreach(Gripper gripper in grippers)
            {
                if(requiredGrabHandParts[i] == gripper.HandPart)
                {
                    requiredResult = true;
                    break;
                }
            }

            if (!requiredResult)
            {
                requiredSkips++;
            }
        }


        int optionalSkips = 0;
        if (requiredSkips == 0)
        {
            for (int i = 0; i < optionalGrabHandParts.Length; i++)
            {
                bool optionalResult = false;
                foreach (Gripper gripper in grippers)
                {
                    if (optionalGrabHandParts[i] == gripper.HandPart)
                    {
                        optionalResult = true;
                        break;
                    }
                }

                if (!optionalResult)
                {
                    optionalSkips++;
                }
            }
        }

        if (requiredSkips > 0 || optionalSkips > maxSkipParts)
        {
            Release();
        }
        else
        {
            Grab(grippers.LastOrDefault().Hand);
        }
    }

    private void Grab(Hand hand)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;

        GripHand = hand;
        GripHand.AddGrableObj(this);

        HandPart[] parts = new HandPart[requiredGrabHandParts.Length + optionalGrabHandParts.Length];
        requiredGrabHandParts.CopyTo(parts, 0);
        optionalGrabHandParts.CopyTo(parts, requiredGrabHandParts.Length);

        GripHand.GripParts = parts;
        transform.parent = hand.Root;
    }

    private void Release()
    {
        rb.useGravity = true;
        transform.parent = defaultParent;

        if (GripHand != null)
        {
            GripHand.RemoveGrableObj(this);
            GripHand.GripParts = null;
        }
    }

    private void GoToStartPosition()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}