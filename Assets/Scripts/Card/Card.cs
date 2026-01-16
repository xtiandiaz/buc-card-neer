using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICard : IDisposable
{
    CardType Type { get; }
    CardType AbstractType { get; }
    CardType? SuitType { get; }
    ISuitModel Suit { get; }
    string Name { get; }

    int Value { get; }
    int LockValue { get; }
    int OriginalValue { get; }
    
    bool IsPlayer { get; }
    bool IsResource { get; }
    bool IsItem { get; }
    bool IsTool { get; }
    bool IsArtifice { get; }
    bool IsMonster { get; }
    bool IsAgent { get; }
    bool IsPirate { get; }
    bool IsMerchant { get; }
    bool IsMeleeWeapon { get; }
    bool IsRangeWeapon { get; }
    bool IsMedicine { get; }
    bool IsRangeTarget { get; }
    bool IsBoarded { get; set; }
    bool IsStashed { get; set; }
    bool IsLocked { get; }

    Vector3 Position { get; }
    Vector3 LocalPosition { get; }
    
    IObservable<Unit> WhenUnlocked { get; }
    IObservable<Unit> WhenDestroyed { get; }
    IObservable<int> WhenHitOrHacked { get; }
    
    void Pick(Vector3 atPosition);
    void Drag(Vector3 toPosition);
    void Drop();
    void Hack(int withValue);
    void Arrange(ArrangementInfo withInfo);
    void Sort(int withRawIndex);
    
    IObservable<Unit> DropAsObservable();
    IObservable<Unit> Reveal();
    IObservable<Unit> Hit(int withValue);
    IObservable<Unit> Clash(ICard withOther, Direction toward);
    IObservable<Unit> OnClashed(int withValue);
    IObservable<Unit> OnShot(int withValue);
    IObservable<Unit> Lodge(LodgingInfo withInfo);
    IObservable<Unit> ArrangeAsObservable(ArrangementInfo withInfo);
    IObservable<Unit> Destroy();
    
    IObservable<Unit> Fling(Vector3 toPosition, Ease withEase, float andDuration);
    void Bounce(Vector3 withVector);
}

public class Card : ICard
{
    public class Factory : PlaceholderFactory<ICardModel, ICardView, Card>
    {
    }
    
    private readonly Subject<int> hittingOrHacking = new Subject<int>();
    private readonly Subject<Unit> destruction = new Subject<Unit>();
    private readonly ReactiveProperty<int> value = new ReactiveProperty<int>();
    private readonly ReactiveProperty<int> lockValue = new ReactiveProperty<int>();
    
    private readonly ICardView view;

    private bool isBoarded;
    private ICardBond bond;
    private ISuitModel suit;
    private ArrangementInfo lastArrangementInfo;

    protected Card(ICardModel model, ICardView view)
    {
        view.Face = model.DealingFace;
        view.ToggleLockVisibility(model.LockValue > 0);
        this.view = view;

        Type = model.Type;
        Suit = model.Suit;

        Value = OriginalValue = model.Value;
        LockValue = model.LockValue;
        WasLocked = IsLocked;
    }

    public CardType Type { get; }
    public CardType AbstractType => IsMonster ? CardType.Monster : Type;
    public CardType? SuitType => Suit?.Type;

    public ISuitModel Suit
    {
        get => suit;
        protected set => suit = view.Suit = value;
    }

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
    public string Name => view.ObjectName;

    public bool IsPlayer => (Type & CardType.Player) != 0;
    public bool IsResource => (Type & CardType.Resource) != 0;
    public bool IsItem => (Type & CardType.Item) != 0;
    public bool IsTool => (Type & CardType.Tool) != 0;
    public bool IsArtifice => (Type & CardType.Artifice) != 0;
    public bool IsMonster => IsResource && (IsLocked || WasLocked);
    public bool IsAgent => (Type & CardType.Agent) != 0;
    public bool IsPirate => (Type & CardType.Pirate) != 0;
    public bool IsMerchant => (Type & CardType.Merchant) != 0;
    public bool IsMeleeWeapon => (Type & CardType.WeaponMelee) != 0;
    public bool IsRangeWeapon => (Type & CardType.WeaponRanged) != 0;
    public bool IsMedicine => (Type & CardType.Medicine) != 0;
    public bool IsRangeTarget => !IsBoarded && 
                                 ((Type & CardType.Pirate) != 0 || IsResource && IsLocked);

    public bool IsBoarded
    {
        get => isBoarded;
        set => isBoarded = view.IsBoarded = value;
    }
    
    public bool IsStashed { get; set; }
    public bool IsLocked => LockValue > 0;

    public Vector3 Position => view.Position;
    public Vector3 LocalPosition => view.LocalPosition;

    public IObservable<Unit> WhenUnlocked => lockValue.Where(value => value <= 0).Take(1).AsUnitObservable();
    public IObservable<Unit> WhenDestroyed => destruction;
    public IObservable<int> WhenHitOrHacked => hittingOrHacking;
    
    private bool WasLocked { get; }

    public void Pick(Vector3 atPosition)
    {
        view.Pick(atPosition);
    }

    public void Drag(Vector3 toPosition)
    {
        view.Drag(toPosition);
    }

    public void Drop()
    {
        view.Arrange(lastArrangementInfo);
    }

    public IObservable<Unit> DropAsObservable()
    {
        return ArrangeAsObservable(lastArrangementInfo);
    }

    public IObservable<Unit> Hit(int withValue)
    {
        return Observable.Create<Unit>(observer =>
        {
            Value -= withValue;
            
            Debug.Log("Hit");
            Debug.Log(Type);
            Debug.Log(withValue);

            if (Value <= 0)
                return Destroy()
                    .Subscribe(observer);
            
            hittingOrHacking.OnNext(withValue);
            
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            
            return Disposable.Empty;
        });
    }

    public void Hack(int withValue)
    {
        LockValue -= withValue;
        
        if (LockValue > 0)
            hittingOrHacking.OnNext(withValue);
    }

    public IObservable<Unit> Reveal()
    {
        return Observable.Create<Unit>(observer =>
        {
            var sequence = view.Reveal()
                .OnComplete(() =>
                {
                    view.Face = CardFace.Front;

                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
            
            return Disposable.Create(() => sequence.Kill());
        });
    }
    
    public IObservable<Unit> Lodge(LodgingInfo withInfo)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (withInfo.Bond != bond)
            {
                bond?.Release(this);
                bond = withInfo.Bond;
            }

            lastArrangementInfo = withInfo.ArrangementInfo;

            var sequence = view.Lodge(withInfo)
                .OnComplete(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
            
            return Disposable.Create(() => sequence.Kill());
        });
    }

    public void Arrange(ArrangementInfo withInfo)
    {
        lastArrangementInfo = withInfo;
        
        view.Arrange(withInfo)
            .OnComplete(() => 
            { 
                view.Sort(withInfo.Index);
            });
    }

    public void Sort(int withRawIndex)
    {
        view.Sort(withRawIndex);
    }

    public IObservable<Unit> ArrangeAsObservable(ArrangementInfo withInfo)
    {
        return Observable.Create<Unit>(observer =>
        {
            lastArrangementInfo = withInfo;
            
            view.Sort(withInfo.Index);
            
            var sequence = view.Arrange(lastArrangementInfo)
                .OnComplete(() => 
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

            return Disposable.Create(() => sequence.Kill());
        });
    }

    public IObservable<Unit> Clash(ICard withOther, Direction toward)
    {
        return view.Clash(toward)
            .Merge(withOther.OnClashed(1))
            .AsSingleUnitObservable();
    }
    
    public IObservable<Unit> OnClashed(int withValue)
    {
        return Observable.Zip(
            view.OnClashed(),
            Observable.Create<Unit>(observer =>
            {
                if (!IsMonster)
                    return Hit(withValue)
                        .Subscribe(observer);
                        
                Debug.Log("OnClashed");
                Debug.Log(Type);
                Debug.Log(withValue);

                Hack(withValue);

                return (LockValue <= 0 ? Destroy() : Observable.ReturnUnit())
                    .Subscribe(observer);
            }))
            .AsSingleUnitObservable();
    }

    public IObservable<Unit> OnShot(int withValue)
    {
        return view.OnShot()
            .ContinueWith(_ => IsResource && IsLocked 
                ? Observable.ReturnUnit().Do(__ => Hack(withValue))
                : Hit(withValue));
    }

    public IObservable<Unit> Fling(Vector3 toPosition, Ease withEase, float andDuration)
    {
        return Observable.Create<Unit>(observer =>
        {
            var sequence = view.Fling(toPosition, withEase, andDuration)
                .OnComplete(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
            
            return Disposable.Create(() => sequence.Kill());
        });
    }

    public void Bounce(Vector3 withVector)
    {
        view.Bounce(withVector);
    }

    public IObservable<Unit> Destroy()
    {
        return Observable.Create<Unit>(observer => 
        {
            bond?.Release(this);

            var tween = view.Fade(0, 0.5f)
                .OnComplete(() =>
                {
                    destruction.OnNext(Unit.Default);
                    destruction.OnCompleted();
                    
                    view.Destroy();
                    Dispose();

                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
            
            return Disposable.Create(() => tween.Kill());
        });
    }

    public virtual void Dispose()
    {
        value.Dispose();
        lockValue.Dispose();
        hittingOrHacking.Dispose();
        destruction.Dispose();
    }
}
