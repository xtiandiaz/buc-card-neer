using UnityEngine;

public abstract class ShipView : MonoBehaviour
{
    [SerializeField] private float height;
    
    public float Height => height;
    
    public void Dock(Vector3 atLocalPosition)
    {
        transform.localPosition = atLocalPosition;
    }
}