using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICard : IDisposable
{
    CardType Type { get; }
    CardType AbstractType { get; }
    CardType? Suit { get; }
    string Name { get; }
    int Index { get; set; }
    
    int Value { get; }
    int LockValue { get; }
    int OriginalValue { get; }
    
    bool IsResource { get; }
    bool IsItem { get; }
    bool IsTool { get; }
    bool IsMonster { get; }
    bool IsPirate { get; }
    bool IsMerchant { get; }
    bool IsMeleeWeapon { get; }
    bool IsRangeWeapon { get; }
    bool IsMedicine { get; }
    bool IsRangeTarget { get; }
    bool IsBoarded { get; set; }
    bool IsStashed { get; set; }
    bool IsLocked { get; }
    bool WasLocked { get; }

    Vector3 LocalPosition { get; }
    
    IObservable<Unit> WhenUnlocked { get; }
    IObservable<Unit> WhenDestroyed { get; }

    void Bind(ICardBond toBond);
    void Pick();
    void Drag(Vector3 byDeltaPosition);
    void Drop();
    void Hack(int withValue);
    
    IObservable<Unit> DropAsObservable();
    IObservable<Unit> Reveal();
    IObservable<Unit> Hit(int withValue);
    IObservable<Unit> Clash(ICard withOther, Direction toward);
    IObservable<Unit> OnClashed(int withValue);
    IObservable<Unit> OnShot(int withValue);
    IObservable<Unit> Arrange(CardArrangement withArrangement);
    IObservable<Unit> Destroy();
}

public class Card : ICard
{
    public class Factory : PlaceholderFactory<ICardModel, ICardView, Card>
    {
    }
    
    private readonly ReactiveProperty<int> value = new ReactiveProperty<int>();
    private readonly ReactiveProperty<int> lockValue = new ReactiveProperty<int>();
    private readonly Subject<Unit> destruction = new Subject<Unit>();
    
    private readonly ICardView view;

    private ICardBond bond;
    private CardArrangement arrangement;

    protected Card(ICardModel model, ICardView view)
    {
        view.Suit = model.Suit;
        view.Face = model.DealingFace;
        view.ToggleLockVisibility(model.LockValue > 0);
        this.view = view;

        Type = model.Type;
        Suit = model.Suit?.Type;
        Name = model.Name;

        Value = OriginalValue = model.Value;
        LockValue = model.LockValue;
        WasLocked = IsLocked;
    }

    public CardType Type { get; }
    public CardType AbstractType => IsMonster ? CardType.Monster : Type;
    public CardType? Suit { get; }
    public string Name { get; }
    public int Index { get; set; }

    public int Value
    {
        get => value.Value;
        protected set => this.value.Value = view.Value = Mathf.Max(value, 0);
    }

    public int LockValue
    {
        get => lockValue.Value;
        private set => lockValue.Value = view.LockValue =  Mathf.Max(value, 0);
    }

    public int OriginalValue { get; }

    public bool IsResource => (Type & CardType.Resource) != 0;
    public bool IsItem => (Type & CardType.Item) != 0;
    public bool IsTool => (Type & CardType.Tool) != 0;
    public bool IsMonster => IsResource && (IsLocked || WasLocked); // TODO provisional; remove
    public bool IsPirate => (Type & CardType.Pirate) != 0;
    public bool IsMerchant => (Type & CardType.Merchant) != 0;
    public bool IsMeleeWeapon => (Type & CardType.WeaponMelee) != 0;
    public bool IsRangeWeapon => (Type & CardType.WeaponRanged) != 0;
    public bool IsMedicine => (Type & CardType.Medicine) != 0;
    public bool IsRangeTarget => !IsBoarded && 
                                 ((Type & CardType.Pirate) != 0 || IsResource && IsLocked);
    public bool IsBoarded { get; set; }
    public bool IsStashed { get; set; }
    public bool IsLocked => LockValue > 0;
    public bool WasLocked { get; }

    public Vector3 LocalPosition => view.LocalPosition;

    public IObservable<Unit> WhenUnlocked => lockValue.Where(value => value <= 0).Take(1).AsUnitObservable();
    public IObservable<Unit> WhenDestroyed => destruction;

    public void Pick()
    {       
        view.Pick();
    }

    public void Drag(Vector3 byDeltaPosition)
    {
        view.Drag(byDeltaPosition);
    }

    public void Drop()
    {
        view.Arrange(arrangement);
    }

    public IObservable<Unit> DropAsObservable()
    {
        return view.ArrangeAsObservable(arrangement, true);
    }

    public IObservable<Unit> Hit(int withValue)
    {
        return Observable.Create<Unit>(observer =>
        {
            Value -= withValue;

            if (Value <= 0)
                return Destroy().Subscribe(observer);
            
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            
            return Disposable.Empty;
        });
    }

    public void Hack(int withValue)
    {
        LockValue -= withValue;
    }

    public IObservable<Unit> Reveal()
    {
        return view.Reveal();
    }

    public IObservable<Unit> Arrange(CardArrangement withArrangement)
    {
        return view.ArrangeAsObservable(withArrangement, false)
            .DoOnSubscribe(() => arrangement = withArrangement);
    }

    public IObservable<Unit> Clash(ICard withOther, Direction toward)
    {
        return view.Clash(toward)
            .Merge(withOther.OnClashed(1))
            .AsSingleUnitObservable();
    }
    
    public IObservable<Unit> OnClashed(int withValue)
    {
        return view.OnClashed()
            .ContinueWith(_ =>
            {
                if (!IsMonster) 
                    return Hit(withValue);
                
                Hack(withValue);

                return LockValue <= 0 ? Destroy() : Observable.ReturnUnit();
            });
    }

    public IObservable<Unit> OnShot(int withValue)
    {
        return view.OnShot()
            .ContinueWith(_ => IsResource && IsLocked 
                ? Observable.ReturnUnit().Do(__ => Hack(withValue))
                : Hit(withValue));
    }

    public void Bind(ICardBond toBond)
    {
        if (toBond == null || toBond == bond)
            return;

        bond?.Release(this);
        bond = toBond;
        
        view.SetParent(bond.Transform);
    }

    public IObservable<Unit> Destroy()
    {
        return Observable.Create<Unit>(observer => 
        {
            bond?.Release(this);

            destruction.OnNext(Unit.Default);
            destruction.OnCompleted();
            
            return view.Fade(0, TimeSpan.FromSeconds(0.3f))
                .DoOnCompleted(() =>
                {
                    view.Destroy();
                    Dispose();
                })
                .Subscribe(observer);
        });
    }

    public void Dispose()
    {
        destruction?.Dispose();
    }
}