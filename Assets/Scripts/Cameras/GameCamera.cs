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
        IBoardLayout boardLayout
    )
    {
        Camera = GetComponent<Camera>();

        var slotSpacing = boardLayout.SlotSpacing;
        var desiredViewWidth = (GameStatics.CardWidth + slotSpacing) 
                               * boardLayout.MaxSupplySlotCount
                               - slotSpacing
                               + boardLayout.Padding.x * 2f;
	
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

    public Vector3 GetWorldPoint(Vector2 fromScreenPoint, float atDepth = 0)
    {
        return Camera.ScreenToWorldPoint(
            new Vector3(fromScreenPoint.x, fromScreenPoint.y, -Camera.transform.localPosition.z + atDepth));
    }

    public void Shake(float withIntensity, TimeSpan duration, int andVibrato = 10)
    {
        shaking?.Kill();
        
        shaking = transform.DOShakePosition((float) duration.TotalSeconds, withIntensity, andVibrato)
            .SetEase(Ease.OutQuart);
    }
}