using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public interface ICamera
{
    Vector3 GetWorldPoint(Vector2 fromScreenPoint);
}

public class BoardCamera : MonoBehaviour, ICamera, IViewportProvider, IWorldPointProvider
{
    [SerializeField] private Camera[] supportingCameras;
    
    private new Camera camera;
    private BoardLayoutSettings layoutSettings;

    [Inject]
    public void Construct(BoardLayoutSettings withLayoutSettings)
    {
        layoutSettings = withLayoutSettings;
        camera = GetComponent<Camera>();
        
        var thisTransform = transform;
        var desiredViewWidth = (layoutSettings.CardSize.x + layoutSettings.CardSpacing.x) 
                               * layoutSettings.MaxCardCountInRow
                               - layoutSettings.CardSpacing.x
                               + layoutSettings.Margins.x * 2f;
        
        thisTransform.position = new Vector3(
            0, 
            0, 
            - (desiredViewWidth / camera.aspect) * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
        
        foreach (var supportCamera in supportingCameras)
            supportCamera.transform.position = thisTransform.position;
    }

    public Viewport GetViewport(float atDepth)
    {
        var distance = Mathf.Abs(camera.transform.localPosition.z - atDepth);
        var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        return new Viewport(frustumHeight * camera.aspect, frustumHeight);
    }

    public Vector3 GetWorldPoint(Vector2 fromScreenPoint)
    {
        return camera.ScreenToWorldPoint(new Vector3(fromScreenPoint.x, fromScreenPoint.y,
            -camera.transform.localPosition.z));
    }
}