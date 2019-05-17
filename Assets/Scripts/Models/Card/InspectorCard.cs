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
        return false;
    }

    public override void Match(ICard withOther)
    {
        
    }
}