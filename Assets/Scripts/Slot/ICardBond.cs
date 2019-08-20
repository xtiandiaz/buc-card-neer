using UnityEngine;

public interface ICardBond
{
    int Index { get; }
    Transform Transform { get; }
    
    void Release(ICard card);
}