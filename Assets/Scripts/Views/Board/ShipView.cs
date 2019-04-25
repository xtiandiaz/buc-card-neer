using System;
using DG.Tweening;
using UnityEngine;

public abstract class ShipView : MonoBehaviour
{
    protected float boardHeight;
    
    [SerializeField] private float height;

    private Sequence transitionSequence;
    private Vector3 outOfViewPosition;
    
    public float Height => height;

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