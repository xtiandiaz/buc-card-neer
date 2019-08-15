using System;
using UniRx;
using UnityEngine;

public interface ICardView
{
    int Value { set; }
    int LockValue { set; }
    CardFace Face { set; }
    ISuitModel Suit { set; }
    
    Sprite FrontCover { set; }
    Sprite BackCover { set; }
    Sprite FrontMotif { set; }
    Sprite BackMotif { set; }
    
    Vector3 Position { get; set; }
    Vector3 LocalPosition { get; }

    void Pick(Vector3 atPosition);
    void Drag(Vector3 toPosition);
    void Arrange(CardArrangement withArrangement);
    void ToggleValueVisibility(bool toValue);
    void ToggleLockVisibility(bool toValue);
    void Destroy();
    
    IObservable<Unit> Clash(Direction toward);
    IObservable<Unit> OnClashed();
    IObservable<Unit> OnShot();
    IObservable<Unit> Reveal();
    IObservable<Unit> Lodge(Transform inTransform, CardArrangement withArrangement, CardArrangementMode andMode);
    IObservable<Unit> ArrangeAsObservable(CardArrangement withArrangement);
    IObservable<Unit> Fade(float toAlphaValue, TimeSpan withDuration);
}