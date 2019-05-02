using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAnimationSettings", menuName = "Game/Settings/Card Animation Settings", order = 1)]
public class CardAnimationSettings : ScriptableObject
{
    [SerializeField] private float flipDurationSeconds = 0.5f;
    [SerializeField] private float liftDepth = 2.5f;
    [SerializeField] private float liftDuration = 0.2f;
    [SerializeField] private float returnToLocationWerePickedDuration = 0.5f;
    [SerializeField] private Ease inEase = Ease.InQuart;
    [SerializeField] private Ease outEase = Ease.OutQuart;
    
    public float FlipDurationSeconds => flipDurationSeconds;
    public float LiftDepth => liftDepth;
    public float LiftDuration => liftDuration;
    public float ReturnToLocationWerePickedDuration => returnToLocationWerePickedDuration;
    public Ease InEase => inEase;
    public Ease OutEase => outEase;
}