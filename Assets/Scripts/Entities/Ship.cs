using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public interface IShip
{
    ISlot Helm { get; }
    ISlot Plank { get; }
    
    IObservable<int> WhenShot { get; }
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
    private readonly ISlot storage;
    private readonly ISlot mount;
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

        this.storage = storage;
        this.mount = mount;
        
        Helm = helm;
        Plank = plank;
        
        this.view = view;
    }
    
    public ISlot Helm { get; }
    public ISlot Plank { get; }

    public IObservable<int> WhenShot => Plank.WhenLodged
        .Where(card => (card.Type & CardType.WeaponRanged) != 0 && card.IsStored)
        .Do(_ => Lock())
        .Delay(TimeSpan.FromSeconds(0.5))
        .Select(Shoot);

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
            return storage;
        
        if ((forType & CardType.Tool) != 0)
            return mount;

        return null;
    }

    private int Shoot(ICard withCard)
    {
        if ((withCard.Type & CardType.WeaponRanged) == 0)
            return 0;

        var weaponValue = withCard.Value;

        //withCard.Destroy();

        return weaponValue;
    }
}
