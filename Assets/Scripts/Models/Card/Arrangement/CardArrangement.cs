using UnityEngine;

public interface ICardArrangement
{
    void Arrange(ICard card, int atIndex, int fromCount);
}

[CreateAssetMenu(fileName = "CardArrangement", menuName = "Game/Card Arrangement", order = 1)]
public class CardArrangement : ScriptableObject, ICardArrangement
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private AnimationCurve offsetFunction;
    
    public void Arrange(ICard card, int atIndex, int fromCount)
    {
        var offsetAtIndex = offset * atIndex;
        
        card.LocalPosition = new Vector3(
            offsetAtIndex.x, 
            offset.y * offsetFunction.Evaluate(atIndex / (float) fromCount), 
            offsetAtIndex.z);
    }
}