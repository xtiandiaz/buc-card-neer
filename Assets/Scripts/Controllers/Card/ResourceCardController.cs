using Zenject;
using UniRx;

public class ResourceCardController : CardController
{
    public class Factory : PlaceholderFactory<ResourceCard, ResourceCardView, ResourceCardController>
    {
    }
    
    private readonly IResourceCard model;
    private readonly IResourceCardView view;
    
    public ResourceCardController(IResourceCard model, IResourceCardView view) : base(model, view)
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