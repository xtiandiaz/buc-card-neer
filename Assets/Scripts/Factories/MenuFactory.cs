using Zenject;

public interface IMenuFactory : IFactory
{
    T Create<T>() where T : IMenu;
}

public class MenuFactory : IMenuFactory
{
    private readonly DiContainer container;
    private readonly IMenuCatalog catalog;

    private MenuFactory(
        DiContainer container,
        IMenuCatalog catalog
        )
    {
        this.container = container;
        this.catalog = catalog;
        
        this.catalog.Index();
    }
    
    public T Create<T>() where T : IMenu
    {
        return container.InstantiatePrefabForComponent<T>(catalog.GetPrefab<T>());
    }
}