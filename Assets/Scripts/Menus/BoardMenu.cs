using UnityEngine;

public interface IBoardMenu : IWorldSpaceMenu
{
    void Parent(Transform child);
}

public class BoardMenu : WorldSpaceMenu, IBoardMenu
{
    public void Parent(Transform child)
    {
        child.SetParent(transform, false);
    }
}