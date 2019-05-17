using UniRx;
using UnityEngine;
using Zenject;

public class PlayerCardController : CardController
{
    public class Factory : PlaceholderFactory<PlayerCard, PlayerCardView, PlayerCardController>
    {
    }
    
    private readonly IPlayerCard model;
    private readonly IPlayerCardView view;
    
    public PlayerCardController(IPlayerCard model, IPlayerCardView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Initialize()
    {
        base.Initialize();
        
        disposables.Add(model.Funds.Subscribe(value => view.CoinsValue = value));
    }
}