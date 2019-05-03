using System;
using UnityEngine;

public interface ICardArrangement
{
    Action<ICard, int, int> Decorate { get; }
    
    Vector3 Transform(Vector3 position, int forIndex, int inCount);
}

[CreateAssetMenu(fileName = "CardArrangement", menuName = "Game/Card Arrangement", order = 1)]
public class CardArrangement : ScriptableObject, ICardArrangement
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private AnimationCurve offsetFunction;
    [SerializeField] [Range(0, 1f)] private float fogDamping = 0.5f;
    [SerializeField] private Color fog;

    public Action<ICard, int, int> Decorate => (card, atIndex, inCount) =>
    {
        var t = atIndex / (float) inCount;
        
        card.Fog(fog, t * (1f - fogDamping));
    };

    public Vector3 Transform(Vector3 position, int forIndex, int inCount)
    {
        var offsetAtIndex = offset * forIndex;
        var t = forIndex / (float) inCount;

        return position + new Vector3(offsetAtIndex.x, offset.y * offsetFunction.Evaluate(t), offsetAtIndex.z);
    }
}