using DG.Tweening;
using UnityEngine;
using Zenject;

public interface ICamera
{
    Vector3 Position { get; }
    
    Rect GetFrustumRect(float forTargetCoordinateZ);
    Vector3 GetWorldPoint(Vector2 fromScreenPoint);
}

public class BoardCamera : MonoBehaviour, ICamera, IInitializable
{
    [SerializeField] private Camera[] supportingCameras;
    
    private new Camera camera;
    private GameSettings settings;
    private Tween scrollingTween;

    public Vector3 Position => transform.position;
    
    [Inject]
    private void Construct(
        GameSettings settings
        )
    {
        this.settings = settings;
    }

    public void Initialize()
    {
        camera = GetComponent<Camera>();
        
        var thisTransform = transform;
        var desiredViewWidth = (settings.CardSize.x + settings.CardSpacing.x) * settings.VisibleCardCountPerRow
                               - settings.CardSpacing.x
                               + settings.BoardMargins.x * 2f;
        
        thisTransform.position = new Vector3(
            0, 
            0, 
            - (desiredViewWidth / camera.aspect) * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
        
        foreach (var supportCamera in supportingCameras)
            supportCamera.transform.position = thisTransform.position;
    }

    public Rect GetFrustumRect(float forTargetCoordinateZ)
    {
        var distance = Mathf.Abs(camera.transform.localPosition.z - forTargetCoordinateZ);
        var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        return new Rect(0, 0, frustumHeight * camera.aspect, frustumHeight);
    }

    public Vector3 GetWorldPoint(Vector2 fromScreenPoint)
    {
        return camera.ScreenToWorldPoint(new Vector3(fromScreenPoint.x, fromScreenPoint.y,
            -camera.transform.localPosition.z));
    }
}
