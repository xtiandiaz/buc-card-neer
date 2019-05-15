using UnityEngine;
using Zenject;

public interface IShipView
{
    float Height { get; }
    Vector3 Position { get; set; }
    ISlotView[] Slots { get; }

    void Dock(Vector3 atLocalPosition);
    void SetSail(Vector3 toLocalPosition);
}

public class ShipView : MonoBehaviour, IShipView
{
    public class Factory : PlaceholderFactory<ShipView>
    {
    }
    
    [SerializeField] private float height;
    [SerializeField] private SlotView[] slots;
    [SerializeField] private ShipAnimationSettings animatorSettings;

    private Transform thisTransform;
    private IShipAnimator animator;

    public float Height => height;
    public ISlotView[] Slots => slots;
    
    public Vector3 Position
    {
        get => thisTransform.position;
        set
        {
            animator.KillMove();
            thisTransform.position = value;
        }
    }

    private void Awake()
    {
        thisTransform = transform;
        
        animator = GetComponent<IShipAnimator>() ?? gameObject.AddComponent<ShipAnimator>();
        animator.Initialize(animatorSettings);
    }

    public void Dock(Vector3 atLocalPosition)
    {
        animator.Dock(atLocalPosition);
    }

    public void SetSail(Vector3 toLocalPosition)
    {
        animator.SetSail(toLocalPosition);
    }
}