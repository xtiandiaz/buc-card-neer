using System;
using UnityEngine;
using Zenject;

public class ItemCardView : CardView
{
    public class Factory : PlaceholderFactory<string, ItemCardView>
    {
    }

    [SerializeField] private SpriteRenderer valueRenderer;
    [SerializeField] private SpriteRenderer itemRenderer;

    protected override void Initialize()
    {
    }
}