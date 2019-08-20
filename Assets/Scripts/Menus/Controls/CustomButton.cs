using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public abstract class CustomButton : Selectable
{
    [SerializeField] private Image background = default;

    public IObservable<Unit> WhenClicked => this.OnPointerClickAsObservable()
        .ThrottleFirst(TimeSpan.FromSeconds(0.2))
        .AsUnitObservable();
    
    protected Color Color
    {
        set => background.color = value;
    }
}