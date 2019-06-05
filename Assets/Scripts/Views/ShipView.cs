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
    public class Factory : PlaceholderFactory<ShipView>
    {
    }
    
    [SerializeField] private SlotView[] slots;
    [SerializeField] private SpriteRenderer hullBackground;
    [SerializeField] private ShipAnimationSettings animatorSettings;
    
    private IShipAnimator animator;
    
    public ISlotView[] Slots => slots;
    public Vector2 HullSize => hullBackground.size;

    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
}