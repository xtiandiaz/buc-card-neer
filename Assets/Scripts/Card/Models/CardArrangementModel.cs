using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public enum CardArrangementMode
{
    Normal,
    Fast
}

public struct CardArrangement
{
    public readonly int index;
    public readonly Vector3 localPosition;
    public readonly float rotationZ;
    public readonly float fogIntensity;
    public readonly Color fogColor;
    
    private readonly float normalDuration;

    public CardArrangement(
        int index, 
        Vector3 localPosition, 
        float rotationZ, 
        float fogIntensity, 
        Color fogColor,
        float normalDuration
        )
    {
        this.index = index;
        this.localPosition = localPosition;
        this.rotationZ = rotationZ;
        this.fogIntensity = fogIntensity;
        this.fogColor = fogColor;
        this.normalDuration = normalDuration;
    }
    
    public float GetDuration(Vector3 fromReferenceLocalPosition, CardArrangementMode forMode)
    {
        if (forMode == CardArrangementMode.Normal)
            return normalDuration;
        
        var placementMargin = Mathf.Clamp(
                                  Vector2.Distance(localPosition, fromReferenceLocalPosition),
                                  0,
                                  GameStatics.HalfCardExtent) / GameStatics.HalfCardExtent;

        return Mathf.Clamp(normalDuration * placementMargin, 0, normalDuration * 0.5f);
    }
}

[CreateAssetMenu(menuName = "Model/Card/Arrangement")]
public class CardArrangementModel : ScriptableObject
{
    [SerializeField] private Vector3 offset = default;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float maxRotationAngle = default;

    [Header("Fog")] 
    [SerializeField] private bool shouldFog = default;
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] [Range(0, 1f)] private float fogIntensity = 0.5f;

    public CardArrangement GetArrangementForIndex(int index, int outOfCount)
    {
        return new CardArrangement(
            index, 
            index * offset, 
            index == 0 ? 0 : Random.Range(-1f, 1f) * maxRotationAngle,
            shouldFog ? fogIntensity * index / outOfCount : 0,
            fogColor,
            duration);
    }
}