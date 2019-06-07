using DG.Tweening;
using UnityEngine;

public interface IShipAnimator
{
    void Initialize(ShipSettings withSettings);
    void Dock(Vector3 atLocalPosition);
    void SetSail(Vector3 toLocalPosition);
    void KillMove();
}

public class ShipAnimator : MonoBehaviour, IShipAnimator
{
    private ShipSettings settings;
    private Sequence transitionSequence;

    public void Initialize(ShipSettings withSettings)
    {
        settings = withSettings;
    }

    public void Dock(Vector3 atLocalPosition)
    {
        ClearTransition();

        transitionSequence.Join(
            Move(atLocalPosition, settings.DockingDuration).SetDelay(settings.DockingDelay));
        transitionSequence.SetEase(settings.DockingEase);
    }

    public void SetSail(Vector3 toLocalPosition)
    {
        ClearTransition();
        
        transitionSequence.Join(Move(toLocalPosition, settings.SailingDuration));
        transitionSequence.SetEase(settings.SailingEase);
    }

    public void KillMove()
    {
        ClearTransition();
    }
    
    private void ClearTransition()
    {
        transitionSequence?.Kill();
        transitionSequence = DOTween.Sequence();
    }

    private Tween Move(Vector3 toLocalPosition, float duringSeconds)
    {
        return transform.DOLocalMove(toLocalPosition, duringSeconds);
    }
}