using UnityEngine;
using Zenject;

public interface IShipView
{
    ISlotView[] Slots { get; }
}

public class ShipView : MonoBehaviour, IShipView
{
    [SerializeField] private SlotView[] slots = default;
    
    [Space]
    [SerializeField] private Transform storage = default;
    [SerializeField] private Transform mount = default;
    
    [Space]
    [SerializeField] private float hullHeight = default;
    [SerializeField] private float hullTopMargin = default;

    public ISlotView[] Slots => slots;
    public float HullHeight => hullHeight;
    public float HullTopMargin => hullTopMargin;

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
        var storagePos = storage.position;
        storagePos.x = -boardModel.CardSize.x - boardModel.SlotSpacing;
        storage.position = storagePos;
        
        var mountPos = mount.position;
        mountPos.x = boardModel.CardSize.x + boardModel.SlotSpacing;
        mount.position = mountPos;
    }
}