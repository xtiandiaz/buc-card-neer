using System;
using UniRx;
using UnityEngine;

public interface IMerchantCard : ICard, IResourceBuyer
{
    IResourceFixation Fixation { get; }
    
    IObservable<IResourceCard> WhenBought { get; }
    
    int GetOffer(IResourceCard forResource);
}

[CreateAssetMenu(menuName = "Game/Card/Merchant")]
public class MerchantCard : Card, IMerchantCard
{
    private readonly Subject<IResourceCard> buying = new Subject<IResourceCard>();

    [SerializeField] private ResourceFixation fixation;

    public override CardType Type => CardType.Merchant;
    public IResourceFixation Fixation => fixation;
    int IResourceBuyer.Coins { get; }

    public IObservable<IResourceCard> WhenBought => buying;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        return withOther is IResourceCard resourceCard && CanBuy(resourceCard);
    }

    public override void Match(ICard withOther)
    {
        if (withOther is IResourceCard resourceCard && CanBuy(resourceCard))
        {
            Buy(resourceCard);
        }
    }

    public bool CanBuy(IResourceCard resource)
    {
        return resource.Owner != null && resource.Owner != (IResourceAgent) this;
    }

    public void Buy(IResourceCard resourceCard)
    {
        // No side-effects (yet)
        
        buying.OnNext(resourceCard);
    }

    public int GetOffer(IResourceCard forResource)
    {
        return forResource.Value 
               * ((fixation.Suit.ResourceType & forResource.ResourceType) != 0 ? Value : 1);
    }
}