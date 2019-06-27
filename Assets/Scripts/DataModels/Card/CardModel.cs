using UnityEngine;

public interface ICardModel
{
    CardType Type { get; }
    SuitModel Suit { get; }
    string Name { get; }
    int Value { get; }
    bool ShouldDisplayValue { get; }
    CardFace DealingFace { get; }
    
    int LockValue { get; }
    
    CardView ViewPrefab { get; }
    
    Sprite FrontCover { get; }
    Sprite BackCover { get; }
    Sprite FrontMotif { get; }
    Sprite BackMotif { get; }
}

[CreateAssetMenu(menuName = "Model/Card")]
public class CardModel : ScriptableObject, ICardModel
{
    [SerializeField] private CardType type;
    [SerializeField] private SuitModel suit;
    [SerializeField] private int value;
    [SerializeField] private bool shouldDisplayValue = true;
    [SerializeField] private CardFace dealingFace = CardFace.Back;
    
    [Space]
    [SerializeField] private int lockValue;
    
    [Space]
    [SerializeField] private Sprite frontCover;
    [SerializeField] private Sprite backCover;
    [SerializeField] private Sprite frontMotif;
    [SerializeField] private Sprite backMotif;
    
    [Space]
    [SerializeField] private CardView viewPrefab;

    public virtual CardType Type => type;
    public SuitModel Suit => suit;
    public string Name => name;
    public int Value => value;
    public bool ShouldDisplayValue => shouldDisplayValue;
    public CardFace DealingFace => dealingFace;
    
    public int LockValue => lockValue;
    
    public CardView ViewPrefab => viewPrefab;
    
    public Sprite FrontCover => frontCover;
    public Sprite BackCover => backCover;
    public Sprite FrontMotif => frontMotif;
    public Sprite BackMotif => backMotif;
}