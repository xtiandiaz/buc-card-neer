using System.Collections.Generic;
using UnityEngine;

public interface ICardArrangement
{
    void Apply(IList<ICard> toCards, Vector3 withAnchorPosition);
}

public abstract class CardArrangement : ScriptableObject, ICardArrangement
{
    [SerializeField] protected CardArrangementSettings settings;

    public abstract void Apply(IList<ICard> toCards, Vector3 withAnchorPosition);

    protected virtual void Apply(ICard toCard, Vector3 withPosition, float byFactor)
    {
        toCard.Flip(settings.Facing);
        toCard.Arrange(withPosition);
    }
}