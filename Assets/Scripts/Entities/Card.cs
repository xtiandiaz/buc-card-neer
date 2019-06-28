using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface ICard : IDisposable
{
    CardType Type { get; }
    string Name { get; }
    int Index { get; set; }
    
    int Value { get; }
    int LockValue { get; }
    int OriginalValue { get; }
    
    bool IsResource { get; }
    bool IsItem { get; }
    bool IsTool { get; }
    bool IsMeleeWeapon { get; }
    bool IsMedicine { get; }
    bool IsRangeTarget { get; }
    bool IsBoarded { get; set; }
    bool IsStored { get; set; }
    bool IsLocked { get; }
    bool WasLocked { get; }
    bool IsExhausted { get; set; }
    
    Vector3 LocalPosition { get; }

    IObservable<Unit> WhenUnlocked { get; }

    void Pick();
    void Drag(Vector3 byDeltaPosition);
    void Hit(int withValue);
    IObservable<Unit> Impact(int withValue);
    void Hack(int withValue);
    void Fog(Color withColor, float byFactor);
    void SetParent(Transform asTransform);
    void Destroy();
    
    IObservable<Unit> Drop();
    IObservable<Unit> Reveal();
    IObservable<Unit> Clash(ICard other, Direction toward);
    IObservable<Unit> Arrange(CardArrangement withArrangement);
}

public class Card : ICard
{
    public class Factory : PlaceholderFactory<ICardModel, ICardView, AudioSource, Card>
    {
    }
    
    private readonly ReactiveProperty<int> value = new ReactiveProperty<int>();
    private readonly ReactiveProperty<int> lockValue = new ReactiveProperty<int>();
    private readonly Subject<Unit> destruction = new Subject<Unit>();
    
    private readonly ICardView view;
    private readonly AudioSource audioSource;

    private bool isExhausted;
    private CardArrangement arrangement;
    private IDisposable fadingAwaySubscription;

    [Inject] private AudioManager audioManager;

    protected Card(ICardModel model, ICardView view, AudioSource audioSource)
    {
        view.Suit = model.Suit;
        view.Face = model.DealingFace;
        view.ToggleLockVisibility(model.LockValue > 0);
        this.view = view;
        
        this.audioSource = audioSource;

        Type = model.Type;
        Name = model.Name;

        Value = OriginalValue = model.Value;
        LockValue = model.LockValue;
        WasLocked = IsLocked;
    }

    public CardType Type { get; }
    public string Name { get; }
    public int Index { get; set; }

    public int Value
    {
        get => value.Value;
        protected set
        {
            this.value.Value = view.Value = value;
            IsExhausted = value <= 0;
        }
    }

    public int LockValue
    {
        get => lockValue.Value;
        private set => lockValue.Value = view.LockValue = value;
    }

    public int OriginalValue { get; }

    public bool IsResource => (Type & CardType.Resource) != 0;
    public bool IsItem => (Type & CardType.Item) != 0;
    public bool IsTool => (Type & CardType.Tool) != 0;
    public bool IsMeleeWeapon => (Type & CardType.WeaponMelee) != 0;
    public bool IsMedicine => (Type & CardType.Medicine) != 0;
    public bool IsRangeTarget => (Type & CardType.Pirate) != 0 || 
                                 (Type & CardType.Resource) != 0 && IsLocked;
    public bool IsBoarded { get; set; }
    public bool IsStored { get; set; }
    public bool IsLocked => LockValue > 0;
    public bool WasLocked { get; }
    public bool IsExhausted
    {
        get => isExhausted;
        set => isExhausted |= value;
    }

    public Vector3 LocalPosition => view.LocalPosition;

    public IObservable<Unit> WhenUnlocked => lockValue.Where(value => value <= 0).Take(1).AsUnitObservable();

    public void Pick()
    {       
        view.Pick();
    }

    public void Drag(Vector3 byDeltaPosition)
    {
        view.Drag(byDeltaPosition);
    }

    public IObservable<Unit> Drop()
    {
        return view.Arrange(arrangement, true);
    }

    public void Hit(int withValue)
    {
        Value -= withValue;
    }

    public IObservable<Unit> Impact(int withValue)
    {
        return view.Impact()
            .Do(_ => Hit(withValue));
    }

    public void Hack(int withValue)
    {
        LockValue -= withValue;
    }

    public IObservable<Unit> Reveal()
    {
        return view.Reveal()
            .DoOnSubscribe(() => audioManager.PlayEvent(AudioEventKey.CardReveal, audioSource));
    }

    public IObservable<Unit> Arrange(CardArrangement withArrangement)
    {
        return view.Arrange(withArrangement, false)
            .DoOnSubscribe(() => arrangement = withArrangement);
    }

    public IObservable<Unit> Clash(ICard other, Direction toward)
    {
        return view.Clash(toward)
            .Merge(other.Impact(1))
            .AsSingleUnitObservable();
    }
    
    public void SetParent(Transform asTransform)
    {
        view.SetParent(asTransform);
    }

    public void Fog(Color withColor, float byFactor)
    {
        view.Fog(withColor, byFactor);
    }

    public void Destroy()
    {
        view.KillMove();

        destruction.OnNext(Unit.Default);
        destruction.OnCompleted();

        fadingAwaySubscription = view.Fade(0, TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ =>
            {
                view.Destroy();
                Dispose();
            });
    }

    public void Dispose()
    {
        fadingAwaySubscription?.Dispose();
        destruction?.Dispose();
    }
}