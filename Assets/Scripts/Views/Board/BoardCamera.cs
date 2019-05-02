using DG.Tweening;
using UnityEngine;
using Zenject;

public interface ICamera
{    
    Rect GetFrustumRect(float forTargetCoordinateZ);
    Vector3 GetWorldPoint(Vector2 fromScreenPoint);
}

public class BoardCamera : MonoBehaviour, ICamera
{
    [SerializeField] private Camera[] supportingCameras;
    
    private new Camera camera;
    private Tween scrollingTween;

    public void Initialize(BoardLayoutSettings withLayoutSettings)
    {
        camera = GetComponent<Camera>();
        
        var thisTransform = transform;
        var desiredViewWidth = (withLayoutSettings.CardSize.x + withLayoutSettings.CardSpacing.x) 
                               * withLayoutSettings.MaxCardCountInRow
                               - withLayoutSettings.CardSpacing.x
                               + withLayoutSettings.Margins.x * 2f;
        
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