public interface IResourceCardView : ICardView
{
}

public class ResourceCardView : CardView, IResourceCardView
{ 
    public override ISuitModel Suit
    {
        set
        {
            base.Suit = value;
            
            cardValue.Color =
                frontMotif.color = value.Color;

            if ((value.Type & CardType.Tool) != 0)
                return;
            
            backCover.Color = value.Color;
        }
    }
}