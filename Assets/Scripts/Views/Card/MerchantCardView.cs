using System;
using UnityEngine;
using Zenject;

public class MerchantCardView : CardView
{
    public class Factory : PlaceholderFactory<string, MerchantCardView>
    {
    }
}