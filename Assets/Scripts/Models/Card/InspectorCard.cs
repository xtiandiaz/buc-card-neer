using UnityEngine;

public interface IInspectorCard : ICard
{
}

[CreateAssetMenu(menuName = "Game/Card/Inspector")]
public class InspectorCard : Card, IInspectorCard
{
    public override CardType Type => CardType.Inspector;
    
    public override bool CanMatch(ICard withOther, ISlot fromSlot)
    {
        return withOther is IResourceCard resourceCard && (resourceCard.ResourceType & ResourceType.Item) != 0;
    }

    public override void Match(ICard withOther)
    {
        if (!(withOther is IResourceCard resourceCard) || (resourceCard.ResourceType & ResourceType.Item) == 0) 
            return;
        
        Value -= withOther.Value;

        withOther.Destroy();
    }

    public override bool CanClash(ICard other)
    {
        return (other.Type & CardType.Pirate) != 0;
    }

    public override bool CanBeImpacted()
    {
        return false;
    }
}