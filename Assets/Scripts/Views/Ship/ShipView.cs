using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public interface IShipView
{
    ShipType Type { get; }
    float Height { get; }
    IEnumerable<ISlotView> Slots { get; }
}

public abstract class ShipView : MonoBehaviour, IShipView
{
    protected float boardHeight;

    [SerializeField] private ShipType type;
    [SerializeField] private float height;
    [SerializeField] private List<SlotView> slots;

    private Sequence transitionSequence;
    private Vector3 outOfViewPosition;

    public ShipType Type => type;
    public float Height => height;
    public IEnumerable<ISlotView> Slots => slots;

    public void Initialize(float withBoardHeight)
    {
        boardHeight = withBoardHeight;
        outOfViewPosition = Vector3.up * (boardHeight + Height * 0.5f);

        transform.localPosition = outOfViewPosition;
    }
    
    public void Dock(Vector3 atLocalPosition, float withDurationInSeconds, float andDelayInSeconds = 0)
    {
        ClearTransition();

        transitionSequence.Join(
            transform.DOLocalMove(atLocalPosition, withDurationInSeconds));

        transitionSequence.SetDelay(andDelayInSeconds);
        transitionSequence.SetEase(Ease.OutQuart);
    }

    public void SetSail(float withDurationInSeconds)
    {
        ClearTransition();
        
        transitionSequence.Join(
            transform.DOLocalMove(outOfViewPosition, withDurationInSeconds));

        transitionSequence.SetEase(Ease.InQuart);
    }

    private void ClearTransition()
    {
        transitionSequence?.Kill();
        transitionSequence = DOTween.Sequence();
    }
}