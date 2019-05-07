using UnityEngine;

public interface ICardArrangementSettings
{
    Vector3 Offset { get; }
    AnimationCurve OffsetFunction { get; }
    Color FogColor { get; }
    float FogDamping { get; }
}

[CreateAssetMenu(fileName = "CardArrangementSettings", menuName = "Game/Settings/Card Arrangement", order = 1)]
public class CardArrangementSettings : ScriptableObject, ICardArrangementSettings
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private AnimationCurve offsetFunction;
    [SerializeField] private CardFace facing;
    
    [Header("Fogging")]
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] [Range(0, 1f)] private float fogDamping = 0.5f;

    public Vector3 Offset => offset;
    public AnimationCurve OffsetFunction => offsetFunction;
    public CardFace Facing => facing;
    public Color FogColor => fogColor;
    public float FogDamping => fogDamping;
}