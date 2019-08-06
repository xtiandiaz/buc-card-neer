using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public interface IGameCamera : IViewportProvider, IWorldPointProvider
{
    Camera Camera { get; }

    void Shake(float withIntensity, TimeSpan duration, int andVibrato = 10);
}

public class GameCamera : MonoBehaviour, IGameCamera
{
    private Tween shaking;
    
    public Camera Camera { get; private set; }

    [Inject]
    private void Initialize(
        IBoardModel fromModel
    )
    {
        Camera = GetComponent<Camera>();

        var slotSpacing = fromModel.SlotSpacing;
        var desiredViewWidth = (fromModel.CardSize.x + slotSpacing) 
                               * fromModel.SupplySlotCount
                               - slotSpacing
                               + fromModel.Padding.x * 2f;
	
        transform.position = new Vector3(
            0, 
            0, 
            - (desiredViewWidth / Camera.aspect) * 0.5f / Mathf.Tan(Camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
    }

    public Viewport GetViewport(float atDepth)
    {
        var distance = Mathf.Abs(Camera.transform.localPosition.z - atDepth);
        var frustumHeight = 2.0f * distance * Mathf.Tan(Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        return new Viewport(frustumHeight * Camera.aspect, frustumHeight);
    }

    public Vector3 GetWorldPoint(Vector2 fromScreenPoint)
    {
        return Camera.ScreenToWorldPoint(
            new Vector3(fromScreenPoint.x, fromScreenPoint.y, -Camera.transform.localPosition.z));
    }

    public void Shake(float withIntensity, TimeSpan duration, int andVibrato = 10)
    {
        shaking?.Kill();
        
        shaking = transform.DOShakePosition((float) duration.TotalSeconds, withIntensity, andVibrato)
            .SetEase(Ease.OutQuart);
    }
}