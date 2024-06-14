using UnityEngine;

public class HandBones : MonoBehaviour
{
    public Transform wrist;
    [Space]
    public Transform indexMetacarpal;
    public Transform indexProximal;
    public Transform indexIntermediate;
    public Transform indexDistal;
    public Transform indexTip;
    [Space]
    public Transform littleMetacarpal;
    public Transform littleProximal;
    public Transform littleIntermediate;
    public Transform littleDistal;
    public Transform littleTip;
    [Space]
    public Transform middleMetacarpal;
    public Transform middleProximal;
    public Transform middleIntermediate;
    public Transform middleDistal;
    public Transform middleTip;
    [Space]
    public Transform palm;
    [Space]
    public Transform ringMetacarpal;
    public Transform ringProximal;
    public Transform ringIntermediate;
    public Transform ringDistal;
    public Transform ringTip;
    [Space]
    public Transform thumbMetacarpal;
    public Transform thumbProximal;
    public Transform thumbDistal;
    public Transform thumbTip;

    public Transform[] ThumbGroup => new[] { thumbTip, thumbDistal, thumbProximal, thumbMetacarpal };
    public Transform[] IndexGroup => new[] { indexTip, indexDistal, indexIntermediate, indexProximal, indexMetacarpal };
    public Transform[] MiddleGroup => new[] { middleTip, middleDistal, middleIntermediate, middleProximal, middleMetacarpal };
    public Transform[] RingGroup => new[] { ringTip, ringDistal, ringIntermediate, ringProximal, ringMetacarpal };
    public Transform[] LittleGroup => new[] { littleTip, littleDistal, littleIntermediate, littleProximal, littleMetacarpal };

    public Transform[] GetGroupByPart(HandPart handPart)
    {
        return handPart switch
        {
            HandPart.Thumb => ThumbGroup,
            HandPart.Index => IndexGroup,
            HandPart.Middle => MiddleGroup,
            HandPart.Ring => RingGroup,
            HandPart.Pinkie => LittleGroup,
            _ => null,
        };
    }
}