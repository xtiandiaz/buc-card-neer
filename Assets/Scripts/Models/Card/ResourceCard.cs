using System;
using System.Reflection;
using UniRx;
using UnityEngine;
using Zenject;

[Flags]
public enum ResourceType
{
    None                = 0,
    Food                = CardType.Food,
    Artifact            = CardType.Artifact,
    Gem                 = CardType.Gem,
    Weapon              = CardType.Weapon,
    
    Item                = Food | Artifact | Gem
}

public interface IResourceCard : ICard
{
    ResourceType ResourceType { get; }
    IResourceAgent Owner { get; }
    Sprite Item { get; }
    ISuit Suit { get; }
    int LockValue { get; }
    bool IsWrapped { get; }
    bool IsLocked { get; }

    IObservable<int> WhenLockValueChanged { get; }
    IObservable<Unit> WhenCollected { get; }
    IObservable<Unit> WhenBought { get; } 
    IObservable<Unit> WhenSold { get; } 
    IObservable<Unit> WhenConsumed { get; }
    IObservable<Unit> WhenUnlocked { get; }

    void OnCollected(IResourceAgent byAgent);
    void OnBought(IResourceAgent byAgent);
    void OnSold(IResourceAgent toAgent);
    void OnConsumed(IResourceAgent byAgent);
    void Unwrap();
    void Unlock();
}

[CreateAssetMenu(fileName = "CardResource", menuName = "Game/Card/Resource", order = 1)]
public class ResourceCard : Card, IResourceCard
{
    private readonly Subject<Unit> collection = new Subject<Unit>();
    private readonly Subject<Unit> buying = new Subject<Unit>();
    private readonly Subject<Unit> selling = new Subject<Unit>();
    private readonly Subject<Unit> consumption = new Subject<Unit>();

    [SerializeField] private Sprite item;
    [SerializeField] private Suit suit;
    [SerializeField] private IntReactiveProperty lockValue = new IntReactiveProperty();

    public override CardType Type => (CardType) ResourceType;
    public IResourceAgent Owner { get; private set; }
    public ResourceType ResourceType => suit.ResourceType;
    public Sprite Item => item;
    public ISuit Suit => suit;
    public bool IsWrapped { get; private set; } = true;
    public bool IsLocked => LockValue > 0;

    public int LockValue
    {
        get => lockValue.Value;
        private set => lockValue.Value = value;
    }

    public IObservable<int> WhenLockValueChanged => lockValue;
    public IObservable<Unit> WhenCollected => collection;
    public IObservable<Unit> WhenBought => buying;
    public IObservable<Unit> WhenSold => selling;
    public IObservable<Unit> WhenConsumed => consumption;
    public IObservable<Unit> WhenUnlocked => lockValue.Where(x => x <= 0).Take(1).AsSingleUnitObservable();

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        if (!IsBoarded || !IsLocked)
            return false;
        
        if (!(withOther is IResourceCard resourceCard) || !resourceCard.IsBoarded)
            return false;
        
        return (resourceCard.ResourceType & ResourceType.Weapon) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (!(withOther is IResourceCard resourceCard) || (resourceCard.ResourceType & ResourceType.Weapon) == 0) 
            return;
        
        LockValue -= withOther.Value;
        
        withOther.Destroy();
    }

    public override void Flip(CardFace toFace)
    {
        if (IsLocked && toFace == CardFace.Front)
            return;
        
        base.Flip(toFace);
    }

    public void OnCollected(IResourceAgent byAgent)
    {
        Unwrap();
        Owner = byAgent;

        collection.OnNext(Unit.Default);
    }

    public void OnBought(IResourceAgent byAgent)
    {
        throw new NotImplementedException();
    }

    public void OnSold(IResourceAgent toAgent)
    {
        selling.OnNext(Unit.Default);
        selling.OnCompleted();
        
        Destroy();
    }

    public void OnConsumed(IResourceAgent byAgent)
    {
        consumption.OnNext(Unit.Default);
        consumption.OnCompleted();

        Destroy();
    }

    public void Unwrap()
    {
        IsWrapped = false;
        
        Flip(CardFace.Front);
    }

    public void Unlock()
    {
        LockValue = 0;
    }
}