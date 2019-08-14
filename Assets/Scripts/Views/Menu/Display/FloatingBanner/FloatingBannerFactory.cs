using UnityEngine;
using Zenject;

public interface IFloatingBannerFactory : IFactory<FloatingBannerType, string, Vector3, FloatingBanner>
{
}

public class FloatingBannerFactory : IFloatingBannerFactory
{
    private readonly FloatingBannerModelCatalog modelCatalog;
    private readonly FloatingBanner.Factory viewFactory;
    private readonly IBoardMenu boardMenu;

    private FloatingBannerFactory(
        FloatingBannerModelCatalog modelCatalog,
        FloatingBanner.Factory viewFactory,
        IBoardMenu boardMenu
        )
    {
        this.modelCatalog = modelCatalog;
        this.modelCatalog.Index();
        
        this.viewFactory = viewFactory;
        this.boardMenu = boardMenu;
    }
    
    public FloatingBanner Create(FloatingBannerType withType, string text, Vector3 atPosition)
    {
        var view = viewFactory.Create();
        
        boardMenu.Parent(view.transform);

        view.RectTransform.anchoredPosition3D = atPosition * boardMenu.ReferencePixelsPerUnit;
        
        view.Initialize(modelCatalog[withType], text);

        return view;
    }
}