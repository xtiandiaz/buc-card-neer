using System.Collections.Generic;
using UnityEngine;

public interface ICardArrangement
{
    void Apply(IList<ICard> toCards, int? fromTotalCount);
}

[CreateAssetMenu(fileName = "CardArrangement", menuName = "Game/Card Arrangement/Default", order = 1)]
public class CardArrangement : ScriptableObject, ICardArrangement
{
    [SerializeField] protected Vector3 offset;
    [SerializeField] protected AnimationCurve offsetFunction;
    [SerializeField] protected CardFace facing;

    public void Apply(IList<ICard> toCards, int? fromTotalCount)
    {
        var countM1 = toCards.Count - 1;
        float total = fromTotalCount ?? toCards.Count;

        for (var i = countM1; i >= 0; i--)
            Apply(toCards[i], i, offset * i, i / total);
    }

    protected virtual void Apply(ICard toCard, int withIndex, Vector3 atPosition, float andTimeStep)
    {
        toCard.Arrange(atPosition, withIndex);
        toCard.Flip(facing);
    }
}