using System;
using UniRx;
using Zenject;

public interface IShip
{
    ISlot Helm { get; }
    ISlot Plank { get; }
    ISlot Storage { get; }
    ISlot Mount { get; }
    
    IObservable<Unit> WhenArmed { get; }
    
    void Lock();
    void Unlock();
    
    ISlot GetStash(CardType forType);
}

public class Ship : IShip
{
    public class Factory : PlaceholderFactory<ISlot, ISlot, ISlot, ISlot, IShipView, Ship>
    {
    }

    private readonly ISlot[] slots;
    private readonly IShipView view;

    private Ship(
        ISlot helm,
        ISlot plank, 
        ISlot storage,
        ISlot mount,
        IShipView view
    )
    {
        slots = new[] {plank, helm, storage, mount};

        Helm = helm;
        Plank = plank;
        Storage = storage;
        Mount = mount;

        this.view = view;
    }
    
    public ISlot Helm { get; }
    public ISlot Plank { get; }
    public ISlot Storage { get; }
    public ISlot Mount { get; }

    public IObservable<Unit> WhenArmed => Plank.WhenLodged
        .Where(card => card.IsRangeWeapon && card.IsStored)
        .AsUnitObservable();

    public void Lock()
    {
        foreach (var slot in slots)
            slot.Lock();
    }

    public void Unlock()
    {
        foreach (var slot in slots)
        {
            if ((slot.Type & SlotType.Player) != 0)
                continue;

            slot.Unlock();
        }
    }

    public ISlot GetStash(CardType forType)
    {
        if ((forType & CardType.Item) != 0)
            return Storage;
        
        if ((forType & CardType.Tool) != 0)
            return Mount;

        return null;
    }
}
