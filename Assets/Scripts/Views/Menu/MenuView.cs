using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public abstract class MenuView : MonoBehaviour
{
    [Inject]
    private void Initialize(Viewport viewport)
    {
        var canvasScaler = GetComponent<CanvasScaler>();
        canvasScaler.scaleFactor = Screen.width / viewport.Size.x / 10f;
    }
}