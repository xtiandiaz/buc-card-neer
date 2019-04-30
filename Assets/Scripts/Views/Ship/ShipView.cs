using DG.Tweening;
using UnityEngine;

public interface IShipView
{
    ShipType Type { get; }
    float Height { get; }
    float ViewportHeight { get; }
    Vector3 Position { get; set; }
    ISlotView[] Slots { get; }

    void Dock(Vector3 atLocalPosition, float withDurationInSeconds, float andDelayInSeconds = 0);
    void SetSail(Vector3 toPosition, float withDurationInSeconds);
}

public abstract class ShipView : MonoBehaviour, IShipView
{
    [SerializeField] private ShipType type;
    [SerializeField] private float height;
    [SerializeField] private SlotView[] slots;

    private Transform thisTransform;
    private Sequence transitionSequence;

    public ShipType Type => type;
    public float Height => height;
    public float ViewportHeight { get; private set; }
    
    public Vector3 Position
    {
        get => thisTransform.position;
        set
        {
            transitionSequence?.Kill();
            thisTransform.position = value;
        }
    }
    
    public ISlotView[] Slots => slots;

    protected virtual void Awake()
    {
        thisTransform = transform;
    }

    public void Dock(Vector3 atLocalPosition, float withDurationInSeconds, float andDelayInSeconds = 0)
    {
        ClearTransition();

        transitionSequence.Join(
            transform.DOLocalMove(atLocalPosition, withDurationInSeconds));

        transitionSequence.SetDelay(andDelayInSeconds);
        transitionSequence.SetEase(Ease.OutQuart);
    }

    public void SetSail(Vector3 toPosition, float withDurationInSeconds)
    {
        ClearTransition();
        
        transitionSequence.Join(
            transform.DOLocalMove(toPosition, withDurationInSeconds));

        transitionSequence.SetEase(Ease.InQuart);
    }

    private void ClearTransition()
    {
        transitionSequence?.Kill();
        transitionSequence = DOTween.Sequence();
    }
}