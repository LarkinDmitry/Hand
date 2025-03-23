using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrableObj : MonoBehaviour
{
    [SerializeField] private HandPart[] toGrabHandParts;
    private Transform defaultParent;
    private List<Gripper> grippers = new();
    private Rigidbody rb;
    private Hand gripHand;

    private Vector3 startPosition;
    private Quaternion startRotation;

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

    private void CheckStatus()
    {
        bool[] result = new bool[toGrabHandParts.Length];

        for (int i = 0; i < toGrabHandParts.Length; i++)
        {
            result[i] = false;
            foreach(Gripper gripper in grippers)
            {
                if(toGrabHandParts[i] == gripper.HandPart)
                {
                    result[i] = true;
                    break;
                }
            }
        }

        if (result.Contains(false))
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
        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        rb.isKinematic = true;

        gripHand = hand;
        gripHand.GripParts = toGrabHandParts;
        transform.parent = hand.Root;
    }

    private void Release()
    {
        rb.isKinematic = false;
        transform.parent = defaultParent;

        if (gripHand != null)
        {
            gripHand.GripParts = null;
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