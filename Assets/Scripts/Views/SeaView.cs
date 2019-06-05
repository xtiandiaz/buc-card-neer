using UnityEngine;
using Zenject;
using DG.Tweening;

public interface ISeaView
{
    float Height { get; }
    Vector3 LocalPosition { get; set; }
    ISlotView[] Slots { get; }
}

public class SeaView : MonoBehaviour, ISeaView
{
    public class Factory : PlaceholderFactory<ISeaView>
    {
    }

    [SerializeField] private SlotView[] slots;
    [Header("Water")]
    [SerializeField] private Transform waterTransform;
    [SerializeField] private Transform waterSurfaceTransform;
    
    private Material oceanMaterial;
    private Sequence projectionSequence;

    public ISlotView[] Slots => slots;
    public float Height => waterSurfaceTransform.localScale.y * Mathf.Sin(waterTransform.rotation.eulerAngles.x * Mathf.Deg2Rad);
    
    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
}