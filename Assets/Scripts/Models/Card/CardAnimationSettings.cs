using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAnimationSettings", menuName = "Game/Settings/Card Animation Settings", order = 1)]
public class CardAnimationSettings : ScriptableObject
{
    [SerializeField] private float moveDuration;
    [SerializeField] private float flipDuration;
    [SerializeField] private float tiltDuration;
    [SerializeField] private float tiltAngle;
    [SerializeField] private float spinDuration;
    [SerializeField] private float liftDeltaZ;
    [SerializeField] private float liftDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private Ease inEase;
    [SerializeField] private Ease outEase;

    public float MoveDuration => moveDuration;
    public float FlipDuration => flipDuration;
    public float TiltDuration => tiltDuration;
    public float TiltAngle => tiltAngle;
    public float SpinDuration => spinDuration;
    public float LiftDeltaZ => liftDeltaZ;
    public float LiftDuration => liftDuration;
    public float FadeDuration => fadeDuration;
    public Ease InEase => inEase;
    public Ease OutEase => outEase;
}