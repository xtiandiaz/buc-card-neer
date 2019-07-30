using UnityEngine;
using Zenject;

public interface IGameCamera
{
    void Initialize(IBoardModel fromModel);
}

public class GameCamera : MonoBehaviour, IGameCamera, IViewportProvider, IWorldPointProvider
{   
    private new Camera camera;
    
    public void Initialize(IBoardModel fromModel)
    {
        camera = GetComponent<Camera>();

        var slotSpacing = fromModel.SlotSpacing;
        var desiredViewWidth = (fromModel.CardSize.x + slotSpacing) 
                               * fromModel.SupplySlotCount
                               - slotSpacing
                               + fromModel.Padding.x * 2f;
	
        transform.position = new Vector3(
            0, 
            0, 
            - (desiredViewWidth / camera.aspect) * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
    }

    public Viewport GetViewport(float atDepth)
    {
        var distance = Mathf.Abs(camera.transform.localPosition.z - atDepth);
        var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        return new Viewport(frustumHeight * camera.aspect, frustumHeight);
    }

    public Vector3 GetWorldPoint(Vector2 fromScreenPoint)
    {
        return camera.ScreenToWorldPoint(
            new Vector3(fromScreenPoint.x, fromScreenPoint.y, -camera.transform.localPosition.z));
    }
}