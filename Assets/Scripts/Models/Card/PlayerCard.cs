using System;
using UniRx;
using UnityEngine;

public interface IPlayerCard : ICard, IResourceTrader, IResourceCollector, IResourceConsumer, IPlunderer
{
    IObservable<int> Health { get; }
    IObservable<int> Funds { get; }
}

[CreateAssetMenu(menuName = "Game/Card/Player")]
public class PlayerCard : Card, IPlayerCard
{
    [SerializeField] private int maxHealthPoints = 14;
    [SerializeField] private IntReactiveProperty coins = new IntReactiveProperty(10);

    public override CardType Type => CardType.Player;

    public int Coins
    {
        get => coins.Value;
        set => coins.Value = Mathf.Max(value, 0);
    }

    public IObservable<int> Health => value;
    public IObservable<int> Funds => coins;
    
    private int HealthPoints
    {
        get => Value;
        set => Value = Math.Min(value, maxHealthPoints);
    }

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        if ((withOther.Type & (CardType.Pirate | CardType.Inspector)) != 0)
            return true;

        if (withOther is IResourceCard resourceCard)
            return resourceCard.IsLocked || CanConsume(resourceCard);

        return false;
    }

    public override void Match(ICard withOther)
    {
        if (withOther is IPirateCard pirateCard)
        {
            Confront(pirateCard);
            return;
        }

        if (withOther is IInspectorCard inspectorCard)
        {
            Confront(inspectorCard);
            return;
        }

        if (!(withOther is IResourceCard resourceCard))
            return;

        if (resourceCard.IsLocked)
            Unlock(resourceCard);
        else if (CanConsume(resourceCard))
            Consume(resourceCard);
    }

    public override bool CanClash(ICard withOther)
    {
        return false;
    }

    public override bool CanBeImpacted()
    {
        return false;
    }

    public bool CanBuy(IResourceCard resource)
    {
        throw new NotImplementedException();
    }
    
    public bool CanSell(IResourceCard resourceCard)
    {
        throw new NotImplementedException();
    }

    public bool CanConsume(IResourceCard resourceCard)
    {
        return (resourceCard.Type & CardType.Medicine) != 0;
    }

    public bool CanCollect(IResourceCard resourceCard)
    {
        return !resourceCard.IsLocked;
    }

    public void Collect(IResourceCard resourceCard)
    {
        throw new NotImplementedException();
    }
    
    public void Buy(IResourceCard resourceCard)
    {
        throw new NotImplementedException();
    }

    public void Sell(IResourceCard resourceCard, IMerchantCard toMerchant)
    {
        Coins += toMerchant.GetOffer(resourceCard);
        
        resourceCard.Destroy();
    }

    public void Consume(IResourceCard resourceCard)
    {
        HealthPoints += resourceCard.Value;

        resourceCard.Value = 0;
    }
    
    public void Plunder(ILootCarrier lootCarrier)
    {
        Coins += lootCarrier.GetLoot();
    }
    
    private void Unlock(IResourceCard resourceCard)
    {
        HealthPoints -= resourceCard.LockValue;
        
        resourceCard.Unlock();
    }

    private void Confront(IPirateCard pirate)
    {
        var pirateHealth = pirate.Value;

        pirate.Value -= Math.Min(HealthPoints, pirateHealth);
        HealthPoints -= pirateHealth;
    }

    private void Confront(IInspectorCard inspector)
    {
        var inspectorPrice = inspector.Value;

        inspector.Value -= Math.Min(Coins, inspectorPrice);
        Coins -= inspectorPrice;
    }
}