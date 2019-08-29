using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IWorldSpaceMenu : IMenu
{
    float ReferencePixelsPerUnit { get; }
}

public abstract class WorldSpaceMenu : Menu, IWorldSpaceMenu
{
    public float ReferencePixelsPerUnit { get; private set; }
    
    [Inject]
    private void Initialize(
        IGameCamera gameCamera,
        Viewport viewport,
        IBoardLayout boardLayout
    )
    {
        GetComponent<Canvas>().worldCamera = gameCamera.Camera;

        var canvasScaler = GetComponent<CanvasScaler>();
        
        canvasScaler.scaleFactor = 
            (boardLayout.Tx > 0.25f ? 0.75f : 1f) * Screen.width / viewport.Size.x / 10f;

        ReferencePixelsPerUnit = canvasScaler.referencePixelsPerUnit;
    }
}