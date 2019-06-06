using System;
using System.Collections.Generic;

public class StorageComparer : IComparer<ICard>
{
    private readonly CardType[] sortingKeys;

    public StorageComparer(CardType[] sortingKeys)
    {
        this.sortingKeys = sortingKeys;
    }
    
    public int Compare(ICard x, ICard y)
    {
        var typeComp = x.Type.CompareTo(y.Type);
        
        return typeComp == 0 
            ? y.BindingTimestamp.CompareTo(x.BindingTimestamp) 
            : Array.IndexOf(sortingKeys, x.Type).CompareTo(Array.IndexOf(sortingKeys, y.Type));
    }
}