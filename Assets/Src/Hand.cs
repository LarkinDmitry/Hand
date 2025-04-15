using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public event Action<bool> OnSwitchGrabState;
    public HandPart[] GripParts { get; set; }
    public BodySide Side { get; private set; }

    private Dictionary<HandPart, Gripper> parts;
    private List<GrableObj> grableObjcts = new();

    public Hand(BodySide side, Gripper palm, Gripper thumb, Gripper index, Gripper middle, Gripper ring, Gripper pinkie)
    {
        Side = side;

        parts = new()
        {
            { HandPart.Palm, palm },
            { HandPart.Thumb, thumb },
            { HandPart.Index, index },
            { HandPart.Middle, middle },
            { HandPart.Ring, ring },
            { HandPart.Pinkie, pinkie }
        };

        foreach (var part in parts)
        {
            part.Value.Iinitialization(part.Key, this);
        }
    }

    public void AddGrableObj(GrableObj grableObj)
    {
        if (!grableObjcts.Contains(grableObj))
        {
            grableObjcts.Add(grableObj);
            OnSwitchGrabState?.Invoke(true);
        }
    }

    public void RemoveGrableObj(GrableObj grableObj)
    {
        if (grableObjcts.Contains(grableObj))
        {
            grableObjcts.Remove(grableObj);
            OnSwitchGrabState?.Invoke(grableObjcts.Count > 0);
        }
    }

    public Transform Root => parts.TryGetValue(HandPart.Palm, out Gripper palm) ? palm.transform : null;
}

public enum BodySide  { None, Left, Right }