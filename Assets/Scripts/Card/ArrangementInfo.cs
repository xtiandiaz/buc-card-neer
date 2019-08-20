using DG.Tweening;
using UnityEngine;

public struct ArrangementInfo
{
    private readonly float duration;
    
    public ArrangementInfo(
        int index, 
        Vector3 localPosition, 
        float rotationZ, 
        Color? fogColor,
        float fogIntensity, 
        CardArrangementMode mode,
        float duration,
        Ease ease
        )
    {
        Index = index;
        LocalPosition = localPosition;
        RotationZ = rotationZ;
        FogColor = fogColor;
        FogIntensity = fogIntensity;
        Mode = mode;
        this.duration = duration;
        Ease = ease;
    }
    
    public int Index { get; }
    public Vector3 LocalPosition { get; }
    public float RotationZ { get; }
    public Color? FogColor { get; }
    public float FogIntensity { get; }
    public CardArrangementMode Mode { get; }
    public Ease Ease { get; }

    public static ArrangementInfo Create(
        CardArrangementModel fromModel,
        LodgingSettings andLodgingSettings,
        int forIndex, 
        int outOfCount)
    {
        return new ArrangementInfo(
            forIndex, 
            forIndex * fromModel.Offset, 
            forIndex == 0 ? 0 : Random.Range(-1f, 1f) * fromModel.MaxRotationAngle,
            fromModel.ShouldFog ? fromModel.FogColor : default,
            fromModel.FogIntensity * forIndex / outOfCount,
            andLodgingSettings.Mode == SlotLodgingMode.Manual ? CardArrangementMode.Fast : CardArrangementMode.Normal,
            andLodgingSettings.Duration,
            andLodgingSettings.Ease);
    }
    
    public float GetDuration(Vector3 fromReferenceLocalPosition)
    {
        if (Mode == CardArrangementMode.Normal)
            return duration;
        
        var placementMargin = Mathf.Clamp(
                                  Vector2.Distance(LocalPosition, fromReferenceLocalPosition),
                                  0,
                                  GameStatics.HalfCardExtent) / GameStatics.HalfCardExtent;

        return Mathf.Clamp(duration * placementMargin, 0, duration * 0.5f);
    }
}