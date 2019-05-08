using UnityEngine;

[CreateAssetMenu(fileName = "SeaCardArrangement", menuName = "Game/Card Arrangement/Sea", order = 1)]
public class SeaCardArrangement : CardArrangement
{
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] [Range(0, 1f)] private float fogDamping = 0.5f;

    protected override void Apply(ICard toCard, int withIndex, float atTime, Vector3 withAnchorPosition)
    {
        var offsetAtIndex = offset * withIndex;
        var arrangedPosition = withAnchorPosition + new Vector3(
                                   offsetAtIndex.x,
                                   offset.y * offsetFunction.Evaluate(atTime),
                                   offsetAtIndex.z);
        
        base.Apply(toCard, withIndex, atTime, arrangedPosition);
        
        toCard.Fog(fogColor, atTime * fogDamping);
    }
}