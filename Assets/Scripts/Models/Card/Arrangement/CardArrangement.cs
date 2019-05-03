using UnityEngine;

public interface ICardArrangement
{
    Vector3 Transform(Vector3 position, int forIndex, int inCount);
}

[CreateAssetMenu(fileName = "CardArrangement", menuName = "Game/Card Arrangement", order = 1)]
public class CardArrangement : ScriptableObject, ICardArrangement
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private AnimationCurve offsetFunction;
    [SerializeField] private Color fog;
    
    //toCard.Fog(fog, offsetT * 0.5f);

    public Vector3 Transform(Vector3 position, int forIndex, int inCount)
    {
        var offsetAtIndex = offset * forIndex;
        var t = forIndex / (float) inCount;

        return position + new Vector3(offsetAtIndex.x, offset.y * offsetFunction.Evaluate(t), offsetAtIndex.z);
    }
}