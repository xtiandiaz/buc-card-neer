using DG.Tweening;
using UnityEngine;

public interface IShipView
{
    ShipType Type { get; }
    float Height { get; }
    float ViewportHeight { get; }
    Vector3 Position { get; set; }
    ISlotView[] Slots { get; }

    void Dock(Vector3 atLocalPosition);
    void SetSail(Vector3 toLocalPosition);
}

public abstract class ShipView : MonoBehaviour, IShipView
{
    [SerializeField] private ShipType type;
    [SerializeField] private float height;
    [SerializeField] private SlotView[] slots;
    [SerializeField] private ShipAnimationSettings animatorSettings;

    private Transform thisTransform;
    private IShipAnimator animator;

    public ShipType Type => type;
    public float Height => height;
    public float ViewportHeight { get; private set; }
    
    public Vector3 Position
    {
        get => thisTransform.position;
        set
        {
            animator.KillMove();
            thisTransform.position = value;
        }
    }
    
    public ISlotView[] Slots => slots;

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