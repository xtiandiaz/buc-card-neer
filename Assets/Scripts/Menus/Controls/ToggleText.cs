using System;
using UniRx;
using UnityEngine;

public class ToggleText : ButtonText
{
    [SerializeField] private BoolReactiveProperty state = new BoolReactiveProperty();

    public IObservable<bool> WhenStateChanged => state.DistinctUntilChanged();

    public void SetState(bool toValue)
    {
        state.Value = toValue;
    }
    
    protected override void Start()
    {
        base.Start();
        
        if (!Application.isPlaying)
            return;

        WhenClicked
            .Subscribe(_ => state.Value = !state.Value)
            .AddTo(this);

        state
            .Subscribe(value =>
            {
                Color = value ? AppColors.ToggleOn : AppColors.ToggleOff;
            })
            .AddTo(this);
    }
}