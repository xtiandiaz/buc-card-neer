using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IStashSlotView : ISlotView
{
    void Initialize(IObservable<Unit> whenSorted);
}

public class StashSlotView : SlotView, IStashSlotView
{
    [SerializeField] private SelectableSprite sortingControl = default;

    [Inject] private IAudioManager audioManager = default;

    public void Initialize(IObservable<Unit> whenSorted)
    {
        sortingControl.WhenTapped
            .Do(_ => audioManager.Play(AudioEventKey.UITapButtonConfirm))
            .SelectMany(whenSorted)
            .Subscribe()
            .AddTo(this);
    }
}