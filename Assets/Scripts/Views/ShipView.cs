using UnityEngine;

public interface IShipView
{
    ISlotView[] Slots { get; }
}

public class ShipView : MonoBehaviour, IShipView
{
    [SerializeField] private SlotView[] slots = default;
    [SerializeField] private float hullHeight = default;

    public ISlotView[] Slots => slots;
    public float HullHeight => hullHeight;

    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
}