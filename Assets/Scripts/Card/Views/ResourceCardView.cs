public interface IResourceCardView : ICardView
{
}

public class ResourceCardView : CardView, IResourceCardView
{ 
    public override ISuitModel Suit
    {
        set
        {
            customizer.ToggleSuitVisibility((value.Type & CardType.Item) != 0);

            customizer.ValueColor =
                customizer.FrontMotifColor = value.Color;

            if ((value.Type & CardType.Tool) != 0)
                return;
            
            base.Suit = value;
            customizer.BackCoverColor = value.Color;
        }
    }
}