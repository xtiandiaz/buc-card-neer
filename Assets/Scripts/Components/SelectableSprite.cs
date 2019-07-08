using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public interface ISelectableSprite
{
    IObservable<Unit> WhenTapped { get; }
}

[RequireComponent(
    typeof(SpriteRenderer), 
    typeof(Collider2D),
    typeof(ObservableEventTrigger))]
public class SelectableSprite : MonoBehaviour, ISelectableSprite
{
    [SerializeField] private ObservableEventTrigger eventTrigger = default;

    public IObservable<Unit> WhenTapped => eventTrigger.OnPointerClickAsObservable().AsUnitObservable();
}