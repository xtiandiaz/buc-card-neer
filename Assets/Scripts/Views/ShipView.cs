using UnityEngine;
using Zenject;

public interface IShipView
{
    Vector3 LocalPosition { get; set; }
    Vector2 HullSize { get; }
    ISlotView[] Slots { get; }
}

public class ShipView : MonoBehaviour, IShipView
{
    [SerializeField] private SlotView[] slots = default;
    [SerializeField] private SpriteRenderer hullBackground = default;

    public ISlotView[] Slots => slots;
    public Vector2 HullSize => hullBackground.size;

    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
}