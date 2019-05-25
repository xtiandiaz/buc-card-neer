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

    public override void Clash(ICard withOther)
    {
        if ((withOther.Type & CardType.Merchant) == 0)
            return;

        Value--;
    }
}