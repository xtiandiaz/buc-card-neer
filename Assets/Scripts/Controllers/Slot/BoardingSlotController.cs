using UniRx;
using Zenject;

public class BoardingSlotController : SlotController
{
    public class Factory : PlaceholderFactory<IBoardingSlot, ISlotView, BoardingSlotController>
    {
    }
    
    protected BoardingSlotController(ISlot model, ISlotView view) : base(model, view)
    {
    }
    
    public override void Initialize()
    {
        base.Initialize();
        
        #region Dismissal

        disposables.Add(view.WhenSwiped
            .Where(dir => dir == Direction.Up)
            .Subscribe(_ => 
            {
                if ((model.Peek()?.Type & CardType.Merchant) != 0)
                    model.Peek()?.Destroy();
            }));

        #endregion
    }
}