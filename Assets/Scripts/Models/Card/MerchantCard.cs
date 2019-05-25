using System;
using UniRx;
using UnityEngine;

public interface IMerchantCard : ICard, IResourceBuyer
{
    ISuit Suit { get; }
    
    IObservable<IResourceCard> WhenBought { get; }
    
    int GetOffer(IResourceCard forResource);
}

[CreateAssetMenu(menuName = "Game/Card/Merchant")]
public class MerchantCard : Card, IMerchantCard
{
    private readonly Subject<IResourceCard> buying = new Subject<IResourceCard>();

    [SerializeField] private Suit suit;

    public override CardType Type => CardType.Merchant;
    public ISuit Suit => suit;
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

    public override void Clash(ICard withOther)
    {
        if ((withOther.Type & CardType.Pirate) == 0)
            return;
        
        Value--;
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
        return forResource.Value * ((Suit.ResourceType & forResource.ResourceType) != 0 ? Value : 1);
    }
}