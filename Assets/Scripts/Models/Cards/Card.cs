using System;
using UnityEngine;

[Flags]
public enum CardType
{
    Player     = 1 << 0,
    Item       = 1 << 1,
    Foe        = 1 << 2,
    Merchant   = 1 << 3,
    Treasure   = 1 << 4
}

public interface ICard
{
    CardType Type { get; }
    Sprite FrontFace { get; }
    Sprite BackFace { get; }
    CardType InteractionMask { get; }

    void Initialize();
    void Flip();
}

public abstract class Card : ScriptableObject, ICard
{
    [SerializeField] private Sprite frontFace;
    [SerializeField] private Sprite backFace;

    public abstract void Initialize();

    protected void Initialize(CardType cardType)
    {
        Type = cardType;
    }

    public CardType Type { get; private set; }
    public Sprite FrontFace => frontFace;
    public Sprite BackFace => backFace;
    
    public CardType InteractionMask => Type;

    public void Flip()
    {}
}