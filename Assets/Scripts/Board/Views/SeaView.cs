using UnityEngine;
using Zenject;
using System.Collections.Generic;

public interface ISeaView
{
    float Height { get; }
    float ZDepth { get; }
    Vector3 LocalPosition { get; set; }

    void ParentAndArrangeSlots(Transform[] transforms, IBoardLayout withLayout);
}

public class SeaView : MonoBehaviour, ISeaView
{    
    [Header("Water")]
    [SerializeField] private Transform waterTransform = default;
    [SerializeField] private Transform waterSurfaceTransform = default;

    [Space]
    [SerializeField] private Transform slotWrapper = default;

    public float Height => waterSurfaceTransform.localScale.y * Mathf.Sin(SurfaceSlope);
    public float ZDepth => waterSurfaceTransform.localScale.y * Mathf.Cos(SurfaceSlope);
    
    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }

    private float SurfaceSlope => (90f - waterTransform.rotation.eulerAngles.x) * Mathf.Deg2Rad;

    public void ParentAndArrangeSlots(Transform[] transforms, IBoardLayout withLayout)
    {
        var positioner = -transforms.Length * 0.5f + 0.5f;
        
        foreach (var slotTransform in transforms)
        {
            slotTransform.SetParent(slotWrapper, false);
            
            slotTransform.localPosition = new Vector3(
                positioner * (GameStatics.CardWidth + withLayout.SlotSpacing),
                0);
            
            positioner += 1f;
        }
    }
}