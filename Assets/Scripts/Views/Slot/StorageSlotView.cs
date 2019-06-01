using System;
using UniRx;
using UnityEngine;

public interface IStorageSlotView : ISlotView
{
    IObservable<Unit> WhenSortingControlTapped { get; }
}

public class StorageSlotView : SlotView, IStorageSlotView
{
    [SerializeField] private SelectableSprite sortingControl;

    public IObservable<Unit> WhenSortingControlTapped => sortingControl.WhenTapped;
}