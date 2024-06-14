using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public HandPart[] GripParts { get; set; }

    private Dictionary<HandPart, Gripper> parts;

    public Hand(Gripper palm, Gripper thumb, Gripper index, Gripper middle, Gripper ring, Gripper pinkie)
    {
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

    public Transform Root => parts.TryGetValue(HandPart.Palm, out Gripper palm) ? palm.transform : null;
}