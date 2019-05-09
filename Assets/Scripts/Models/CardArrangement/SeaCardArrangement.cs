using UnityEngine;

[CreateAssetMenu(fileName = "SeaCardArrangement", menuName = "Game/Card Arrangement/Sea", order = 1)]
public class SeaCardArrangement : CardArrangement
{
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] [Range(0, 1f)] private float fogDamping = 0.5f;

    protected override void Apply(ICard toCard, int withIndex, Vector3 atPosition, float andTimeStep)
    {
        var offsetAtIndex = offset * withIndex;
        var arrangedPosition = new Vector3(offsetAtIndex.x, offset.y * offsetFunction.Evaluate(andTimeStep), offsetAtIndex.z);
        
        base.Apply(toCard, withIndex, arrangedPosition, andTimeStep);
        
        toCard.Fog(fogColor, andTimeStep * fogDamping);
    }
}