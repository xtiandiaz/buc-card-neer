using UnityEngine;

[CreateAssetMenu(menuName = "Model/Card/Arrangement")]
public class CardArrangementModel : ScriptableObject
{
    [SerializeField] private Vector3 offset = default;
    [SerializeField] private float maxRotationAngle = default;

    [Header("Fog")] 
    [SerializeField] private bool shouldFog = default;
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] [Range(0, 1f)] private float fogIntensity = 0.5f;

    public Vector3 Offset => offset;
    public float MaxRotationAngle => maxRotationAngle;

    public bool ShouldFog => shouldFog;
    public Color FogColor => fogColor;
    public float FogIntensity => fogIntensity;
}