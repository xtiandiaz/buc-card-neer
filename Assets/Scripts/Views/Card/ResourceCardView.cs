public interface IResourceCardView : ICardView
{
}

public class ResourceCardView : CardView, IResourceCardView
{ 
    public override ISuitModel Suit
    {
        set
        {
            if (suit != null)
                suit.ToggleVisibility((value.Type & CardType.Item) != 0);
            
            cardValue.Color =
                frontMotif.color = value.Color;

            if ((value.Type & CardType.Tool) != 0)
                return;
            
            base.Suit = value;
            backCover.Color = value.Color;
        }
    }
}