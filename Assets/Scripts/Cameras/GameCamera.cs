using UnityEngine;
using Zenject;

public interface IGameCamera
{
    void Initialize(BoardLayoutSettings withLayoutSettings);
}

public class GameCamera : MonoBehaviour, IGameCamera, IViewportProvider, IWorldPointProvider
{   
    private new Camera camera;
    
    public void Initialize(BoardLayoutSettings withLayoutSettings)
    {
        camera = GetComponent<Camera>();
        
        camera.orthographicSize = ((withLayoutSettings.CardSize.x + withLayoutSettings.CardSpacing.x) 
                                  * withLayoutSettings.MaxCardCountInRow
                                  - withLayoutSettings.CardSpacing.x
                                  + withLayoutSettings.Margins.x * 2f) 
                                  * Screen.height / Screen.width * 0.5f;
    }

    public Viewport GetViewport(float atDepth)
    {
        var height = camera.orthographicSize * 2f;
        
        return new Viewport(Screen.width * height / Screen.height, height);
    }

    public Vector3 GetWorldPoint(Vector2 fromScreenPoint)
    {
        return camera.ScreenToWorldPoint(
            new Vector3(fromScreenPoint.x, fromScreenPoint.y, -camera.transform.localPosition.z));
    }
}