using System.Collections.Generic;
using UnityEngine;

public class SeaCardArrangement : CardArrangement
{
    public override void Apply(IList<ICard> toCards, Vector3 withAnchorPosition)
    {
        float count = toCards.Count;
        
        for (var i = 0; i < toCards.Count; i++)
        {
            var offsetAtIndex = settings.Offset * i;
            var t = i / count;
            var arrangedPosition = withAnchorPosition + new Vector3(
                                       offsetAtIndex.x,
                                       settings.Offset.y * settings.OffsetFunction.Evaluate(t),
                                       offsetAtIndex.z);
            
            Apply(toCards[i], arrangedPosition, t);
        }
    }

    protected override void Apply(ICard toCard, Vector3 withPosition, float byFactor)
    {
        base.Apply(toCard, withPosition, byFactor);
        
        toCard.Fog(settings.FogColor, byFactor);
    }
}