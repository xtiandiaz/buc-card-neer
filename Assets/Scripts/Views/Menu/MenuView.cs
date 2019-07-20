using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public abstract class MenuView : MonoBehaviour
{
    [Inject]
    private void Initialize(
        Viewport viewport,
        IBoardModel boardModel
        )
    {
        var canvasScaler = GetComponent<CanvasScaler>();
        canvasScaler.scaleFactor =( boardModel.Tx > 0.25f ? 0.75f : 1f) * Screen.width / viewport.Size.x / 10f;
    }
}