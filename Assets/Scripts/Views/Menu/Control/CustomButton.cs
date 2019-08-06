using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public abstract class CustomButton : Selectable
{
    public IObservable<Unit> WhenClicked => this.OnPointerClickAsObservable()
        .ThrottleFirst(TimeSpan.FromSeconds(0.2))
        .AsUnitObservable();
}