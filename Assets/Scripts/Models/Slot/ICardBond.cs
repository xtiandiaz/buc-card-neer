using UnityEngine;

public interface ICardBond
{
    Transform TransformBond { get; }
    
    void Release(ICard card);
}