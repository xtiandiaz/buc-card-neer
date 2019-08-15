using System;
using UnityEngine;
using DG.Tweening;
using Zenject;

public interface ISeaView
{
    float Height { get; }
    float Depth { get; }
    Vector3 LocalPosition { get; set; }
    
    ISlotView[] Slots { get; }
}

public class SeaView : MonoBehaviour, ISeaView
{
    [SerializeField] private SlotView[] slots = default;
    
    [Header("Water")]
    [SerializeField] private Transform waterTransform = default;
    [SerializeField] private Transform waterSurfaceTransform = default;
    
    private Material oceanMaterial;
    private Sequence projectionSequence;

    public float Height => waterSurfaceTransform.localScale.y * Mathf.Sin(SurfaceSlope);
    public float Depth => waterSurfaceTransform.localScale.y * Mathf.Cos(SurfaceSlope);

    private float SurfaceSlope => (90f - waterTransform.rotation.eulerAngles.x) * Mathf.Deg2Rad;
    
    public ISlotView[] Slots => slots;
    
    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }

    [Inject]
    private void Initialize(
        IBoardModel boardModel
        )
    {
        var positioner = -slots.Length * 0.5f + 0.5f;
        
        foreach (var slot in slots)
        {
            var slotPos = slot.Transform.localPosition;

            slotPos.x = positioner * (GameStatics.CardWidth + boardModel.SlotSpacing);
            
            slot.Transform.localPosition = slotPos;
            
            positioner += 1f;
        }
    }
}