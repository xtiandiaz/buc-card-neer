using UnityEngine;

public interface IMerchantCard : ICard
{
    IResourceFixation Fixation { get; }
}

[CreateAssetMenu(menuName = "Game/Card/Merchant")]
public class MerchantCard : Card, IMerchantCard
{
    [SerializeField] private ResourceFixation fixation;
    
    public override CardType Type => CardType.Merchant;
    public IResourceFixation Fixation => fixation;

    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        if (!IsBoarded)
            return false;
        
        return withOther is IResourceCard resourceCard && resourceCard.IsAcquired;
    }

    public override void Match(ICard withOther)
    {
        if (withOther is IResourceCard resourceCard && resourceCard.IsAcquired)
        {
            resourceCard.Sell((fixation.Suit.ResourceType & resourceCard.ResourceType) != 0 
                ? fixation.Degree
                : 1);
        }
    }
}