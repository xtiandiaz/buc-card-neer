using UnityEngine;
using Zenject;

public class CardController
{
    public class Factory : PlaceholderFactory<ICard, ICardView, CardController>
    {       
    }
    
    private CardController(ICard model, ICardView view)
    {
        Model = model;
        View = view;
    }
    
    public ICard Model { get; }
    private ICardView View { get; }

    public void Locate(Vector3 localPosition)
    {
        View.LocalPosition = localPosition;
    }
    
    public void Destroy()
    {
        View.Destroy();
    }
}