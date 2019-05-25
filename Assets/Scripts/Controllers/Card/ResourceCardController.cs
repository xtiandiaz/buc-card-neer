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
        view.Container = model.Container;

        disposables.Add(model.WhenLockValueChanged.Subscribe(lockValue =>
        {
            view.LockValue = lockValue;
            view.ToggleLock(lockValue > 0);
        }));
        
        disposables.Add(model.WhenUnlocked.Subscribe(_ => model.Unwrap()));
    }
}