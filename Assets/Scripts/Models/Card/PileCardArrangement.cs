using System.Collections.Generic;
using UnityEngine;

public interface IPileCardArrangement
{
    void OnSorted(IList<ICard> contents);
    void Apply(IList<ICard> toContents, int? fromTotal);
}

[CreateAssetMenu(fileName = "CardArrangement", menuName = "Game/Card Arrangement/Default", order = 1)]
public class PileCardArrangement : ScriptableObject, IPileCardArrangement
{
    [SerializeField] protected Vector3 offset;

    [Header("Fog")] 
    [SerializeField] private bool shouldFog;
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] [Range(0, 1f)] private float fogIntensity = 0.5f;

    public void OnSorted(IList<ICard> contents)
    {
        Apply(contents, contents.Count, CardMoveType.Sorting);
    }

    public void Apply(IList<ICard> toContents, int? fromTotal)
    {
        Apply(toContents, fromTotal, CardMoveType.Lodging);
    }
    
    private void Apply(IList<ICard> toContents, int? fromTotal, CardMoveType withMoveType)
    {
        var countM1 = toContents.Count - 1;
        float total = fromTotal ?? toContents.Count;

        for (var i = countM1; i >= 0; i--)
            Apply(toContents[i], i, offset * i, i / total, withMoveType);
    }

    private void Apply(ICard toCard, int withIndex, Vector3 atPosition, float andTimeStep, CardMoveType withMoveType)
    {
        if (shouldFog)
            toCard.Fog(fogColor, andTimeStep * fogIntensity);

        toCard.Index = withIndex;

        toCard.Move(atPosition, withMoveType);
    }
}