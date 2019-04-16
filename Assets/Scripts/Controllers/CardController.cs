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

    public void Arrange(Vector3 slotPosition, int indexInPile, int inverseIndexInPile)
    {
        View.Arrange(
            slotPosition + indexInPile * Vector3.up * settings.CardOffsetInPile.y, 
            inverseIndexInPile);
    }
    
    public void Destroy()
    {
        View.Destroy();
    }
}