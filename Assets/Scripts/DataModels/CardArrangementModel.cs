using UnityEngine;
using Random = UnityEngine.Random;

public struct CardArrangement
{
    public readonly int index;
    public readonly Vector3 localPosition;
    public readonly float rotationZ;
    public readonly float fogIntensity;
    public readonly Color fogColor;

    public CardArrangement(int index, Vector3 localPosition, float rotationZ, float fogIntensity, Color fogColor)
    {
        this.index = index;
        this.localPosition = localPosition;
        this.rotationZ = rotationZ;
        this.fogIntensity = fogIntensity;
        this.fogColor = fogColor;
    }
}

[CreateAssetMenu(menuName = "Model/Card Arrangement")]
public class CardArrangementModel : ScriptableObject
{
    [SerializeField] private Vector3 offset = default;
    [SerializeField] private float maxRotationAngle = default;
    [SerializeField] private bool shouldUseReverseIndices = default;

    [Header("Fog")] 
    [SerializeField] private bool shouldFog = default;
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] [Range(0, 1f)] private float fogIntensity = 0.5f;

    public Vector3 Offset => offset;
    public bool ShouldUseReverseIndices => shouldUseReverseIndices;

    public CardArrangement GetArrangementForIndex(int index, int outOfCount)
    {
        return new CardArrangement(
            index, 
            index * offset, 
            index == 0 ? 0 : Random.Range(-1f, 1f) * maxRotationAngle,
            shouldFog ? fogIntensity * index / outOfCount : 0,
            fogColor);
    }
}