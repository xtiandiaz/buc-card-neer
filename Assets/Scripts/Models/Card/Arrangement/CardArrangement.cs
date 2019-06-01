using System.Collections.Generic;
using UnityEngine;

public enum CardArrangementMode
{
    Transitional,
    Immediate
}

public interface ICardArrangement
{
    void Apply(IList<ICard> toCards, int? fromTotalCount, CardArrangementMode withMode);
}

[CreateAssetMenu(fileName = "CardArrangement", menuName = "Game/Card Arrangement/Default", order = 1)]
public class CardArrangement : ScriptableObject, ICardArrangement
{
    [SerializeField] protected Vector3 offset;
    [SerializeField] protected AnimationCurve offsetFunction;
    [SerializeField] protected CardFace facing;
    [SerializeField] protected bool shouldTopMostRemainInPlace;

    public void Apply(IList<ICard> toCards, int? fromTotalCount, CardArrangementMode withMode)
    {
        var countM1 = toCards.Count - 1;
        float total = fromTotalCount ?? toCards.Count;

        for (var i = countM1; i >= 0; i--)
            Apply(toCards[i], i, offset * (shouldTopMostRemainInPlace ? countM1 - i : i), i / total, withMode);
    }

    protected virtual void Apply(ICard toCard, int withIndex, Vector3 atPosition, float andTimeStep, CardArrangementMode withMode)
    {
        toCard.Arrange(atPosition, withIndex, withMode);
    }
}