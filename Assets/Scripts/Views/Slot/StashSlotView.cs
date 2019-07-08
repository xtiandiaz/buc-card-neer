using System;
using UniRx;
using UnityEngine;

public interface IStashSlotView : ISlotView
{
    IObservable<Unit> WhenSortingControlTapped { get; }
}

public class StashSlotView : SlotView, IStashSlotView
{
    [SerializeField] private SelectableSprite sortingControl = default;

    public IObservable<Unit> WhenSortingControlTapped => sortingControl.WhenTapped;
}