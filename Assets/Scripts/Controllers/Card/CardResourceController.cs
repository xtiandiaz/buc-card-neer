using Zenject;
using UniRx;

public class CardResourceController : CardController
{
    public class Factory : PlaceholderFactory<CardResource, CardResourceView, CardResourceController>
    {
    }
    
    private readonly ICardResource model;
    private readonly ICardResourceView view;
    
    public CardResourceController(ICardResource model, ICardResourceView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Initialize()
    {
        base.Initialize();

        view.Suit = model.Suit;
        view.Item = model.Item;
        
        disposables.Add(model.LockValueAsObservable.Subscribe(lockValue =>
        {
            view.LockValue = lockValue;
            view.ToggleLock(lockValue > 0);
        }));
        
        // For now, Resources are automatically purchased when boarded and unlocked:
        disposables.Add(model.WhenBoarded
            .SelectMany(_ => model.WhenUnlocked)
            .Subscribe(_ => model.Purchase()));
    }
}