using UnityEngine;

public interface IMerchantCardView : ICardView
{
    IResourceFixation Fixation { set; }
}

public class MerchantCardView : CardView, IMerchantCardView
{
    [SerializeField] private CardGraphic fixationGraphic;
    [SerializeField] private CardLabel fixationLabel;

    public IResourceFixation Fixation
    {
        set
        {
            fixationGraphic.Sprite = value.Suit.Icon;
            fixationLabel.SetValue($"{value.Degree}x");
        }
    }
}