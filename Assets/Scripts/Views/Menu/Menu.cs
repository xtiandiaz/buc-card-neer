using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IMenu
{
}

[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public abstract class Menu : MonoBehaviour, IMenu
{
    [Inject]
    private void Initialize(
        IGameCamera gameCamera,
        Viewport viewport,
        IBoardModel boardModel
    )
    {
        GetComponent<Canvas>().worldCamera = gameCamera.Camera;

        GetComponent<CanvasScaler>().scaleFactor = 
            (boardModel.Tx > 0.25f ? 0.75f : 1f) * Screen.width / viewport.Size.x / 10f;
    }
}