using DG.Tweening;
using UnityEngine;
using Zenject;

public interface IBoardCamera
{
    Vector3 Position { get; }
    
    Rect GetFrustumRect(float forTargetCoordinateZ);
}

public class BoardCamera : MonoBehaviour, IBoardCamera
{
    [SerializeField] private Camera camera;
    
    private GameSettings settings;
    private Tween scrollingTween;

    public Vector3 Position => transform.position;
    
    [Inject]
    private void Construct(
        GameSettings settings
        )
    {
        this.settings = settings;
        
        var thisTransform = transform;
        var desiredViewWidth = (settings.DisplacementUnit.x) * settings.VisibleCardCountPerRow + settings.CardSpacing.x;
        
        thisTransform.position = new Vector3(
            0, 
            0, 
            - (desiredViewWidth / camera.aspect) * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
    }

    public Rect GetFrustumRect(float forTargetCoordinateZ)
    {
        var distance = Mathf.Abs(camera.transform.localPosition.z - forTargetCoordinateZ);
        var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        return new Rect(0, 0, frustumHeight * camera.aspect, frustumHeight);
    }

    public void Move(Coordinates toCoordinates)
    {
        scrollingTween?.Kill();
        scrollingTween = transform.DOMove(
                new Vector3(
                    toCoordinates.x * settings.DisplacementUnit.x,
                    toCoordinates.y * settings.DisplacementUnit.y,
                    transform.position.z),
                settings.MoveDurationInSeconds)
            .SetEase(Ease.InOutQuart);
    }
}
