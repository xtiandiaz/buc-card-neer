using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IStashSlotView : ISlotView
{
    IObservable<Unit> WhenSorted { get; set; }
    IObservable<int> WhenCardCountChanged { get; set; }
}

public class StashSlotView : SlotView, IStashSlotView
{
    [SerializeField] private SelectableSprite sortingControl = default;

    private IAudioManager audioManager;

    public IObservable<Unit> WhenSorted { get; set; }
    public IObservable<int> WhenCardCountChanged { get; set; }

    [Inject]
    private void Initialize(IAudioManager audioManager)
    {
        this.audioManager = audioManager;
    }

    private void Start()
    {
        sortingControl.WhenTapped
            .Do(_ => audioManager.Play(AudioEventKey.UITapButtonConfirm))
            .SelectMany(WhenSorted)
            .Subscribe()
            .AddTo(this);

        WhenCardCountChanged
            .DoOnSubscribe(() => sortingControl.gameObject.SetActive(false))
            .Subscribe(count => sortingControl.gameObject.SetActive(count > 1))
            .AddTo(this);
    }
}