using System;
using UnityEngine;

public interface ICardArrangementSettings
{
    bool ShouldFog { get; }
    Color FogColor { get; }
    float FogDamping { get; }
    
    Vector3 Transform(Vector3 position, int forIndex, int inCount);
}

[CreateAssetMenu(fileName = "CardArrangementSettings", menuName = "Game/Settings/Card Arrangement", order = 1)]
public class CardArrangementSettings : ScriptableObject, ICardArrangementSettings
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private AnimationCurve offsetFunction;
    
    [Header("Fogging")]
    [SerializeField] private bool shouldFog;
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] [Range(0, 1f)] private float fogDamping = 0.5f;

    public bool ShouldFog => shouldFog;
    public Color FogColor => fogColor;
    public float FogDamping => fogDamping;

    public Vector3 Transform(Vector3 position, int forIndex, int inCount)
    {
        var offsetAtIndex = offset * forIndex;
        var t = forIndex / (float) inCount;

        return position + new Vector3(offsetAtIndex.x, offset.y * offsetFunction.Evaluate(t), offsetAtIndex.z);
    }
}