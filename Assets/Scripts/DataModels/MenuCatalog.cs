using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuCatalog
{
    void Index();
    Menu GetPrefab<T>() where T : IMenu;
}

[CreateAssetMenu(menuName = "Model/Menu Catalog")]
public class MenuCatalog : ScriptableObject, IMenuCatalog
{
    private Dictionary<Type, Menu> prefabIndex = new Dictionary<Type, Menu>();

    public void Index()
    {
        //...
    }

    public Menu GetPrefab<T>() where T : IMenu
    {
        return prefabIndex[typeof(T)];
    }
}