using JetBrains.Annotations;
using UnityEngine;

public class CardCustomizer : MonoBehaviour
{
    [SerializeField] protected CardValue cardValue = default;
    
    [CanBeNull] [SerializeField] protected CardValue cardValue2 = default;
    [CanBeNull]
    [SerializeField] protected CardValue lockValue = default;
    [CanBeNull]
    [SerializeField] protected CardValue cardName = default;
    
    [CanBeNull]
    [SerializeField] protected Suit suit = default;
    
    [Space]
    [SerializeField] protected CardCover frontCover = default;
    [SerializeField] protected CardCover backCover = default;
    [SerializeField] private Transform covers = default;
    
    [CanBeNull]
    [SerializeField] protected SpriteRenderer frontMotif = default;
    [CanBeNull] 
    [SerializeField] protected SpriteRenderer backMotif = default;
    
    private CardFace face;
    
    public int Value
    {
        set
        {
            cardValue?.SetValue(value);
            cardValue2?.SetValue(value);
        }
    }

    public Color ValueColor
    {
        set
        {
            if (cardValue != null)
                cardValue.Color = value;
            
            if (cardValue2 != null)
                cardValue2.Color = value;
        }
    }

    public int LockValue
    {
        set
        {
            lockValue?.SetValue(value);
        }
    }

    public string CardName
    {
        set 
        { 
            cardName?.SetValue(value); 
        }
    }

    public ISuitModel Suit
    {
        set
        {
            if (suit != null && value != null) 
                suit.Customize(value);
        }
    }
    
    public Sprite FrontCover
    {
        set => frontCover.Cover = value;
    }
    
    public Color FrontCoverColor
    {
        set => frontCover.Color = value;
    }

    public Sprite BackCover
    {
        set => backCover.Cover = value;
    }
    
    public Color BackCoverColor
    {
        set => backCover.Color = value;
    }
    
    public Sprite FrontMotif
    {
        set
        {
            if (frontMotif != null)
                frontMotif.sprite = value;
        }
    }

    public Color FrontMotifColor
    {
        set
        {
            if (frontMotif != null)
                frontMotif.color = value;
        }
    }

    public Sprite BackMotif
    {
        set
        {
            if (backMotif != null)
                backMotif.sprite = value;
        }
    }
    
    public CardFace Face
    {
        set
        {
            var eulerAngles = covers.eulerAngles;
            eulerAngles.y = value == CardFace.Front ? 0 : 180f;
            
            covers.eulerAngles = eulerAngles;
            face = value;
            
            frontCover.ToggleVisibility(face == CardFace.Front);
            backCover.ToggleVisibility(face == CardFace.Back);
        }
    }
    
    public void ToggleValueVisibility(bool toValue)
    {
        cardValue?.ToggleVisibility(toValue);
        cardValue2?.ToggleVisibility(toValue);
    }
    
    public void ToggleLockVisibility(bool toValue)
    {
        lockValue?.ToggleVisibility(toValue);
    }

    public void ToggleSuitVisibility(bool toValue)
    {
        suit?.ToggleVisibility(toValue);
    }
}
