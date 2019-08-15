using UniRx;
using Zenject;

public class StashSlot : Slot
{
    public new class Factory : PlaceholderFactory<ISlotModel, IStashSlotView, StashSlot>
    {
    }
    
    protected StashSlot(ISlotModel model, IStashSlotView view) 
        : base(model, view)
    {
        view.WhenSorted = Observable.Create<Unit>(observer => 
        { 
            pile.InsertReverse(pile.Pop());
            
            Lock();
            
            return ArrangeAsObservable()
                .DoOnCompleted(Unlock)
                .Subscribe(observer);
        });

        view.WhenCardCountChanged = pile.WhenCountChanged;
    }
}