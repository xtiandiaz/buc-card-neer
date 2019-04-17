using UnityEngine;
using Zenject;

public class CardController
{
    public class Factory : PlaceholderFactory<ICard, ICardView, CardController>
    {       
    }

    private readonly GameSettings settings;
    
    private CardController(
        ICard model, 
        ICardView view, 
        GameSettings settings
        )
    {
        Model = model;
        View = view;
        
        this.settings = settings;
    }
    
    public ICard Model { get; }
    private ICardView View { get; }

    public void Arrange(Vector3 forSlotPosition, int withStackIndex, int andInverseStackIndex)
    {
        View.Arrange(
            forSlotPosition + withStackIndex * Vector3.up * settings.CardOffsetInPile.y, 
            withStackIndex, 
            andInverseStackIndex);
    }
    
    public void Destroy()
    {
        View.Destroy();
    }
}