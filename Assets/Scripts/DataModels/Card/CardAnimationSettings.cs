using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAnimationSettings", menuName = "Game/Settings/Card Animation Settings", order = 1)]
public class CardAnimationSettings : ScriptableObject
{
    [SerializeField] private float moveDuration = default;
    [SerializeField] private float flipDuration = default;
    [SerializeField] private float tiltDuration = default;
    [SerializeField] private float tiltAngle = default;
    [SerializeField] private float spinDuration = default;
    [SerializeField] private float liftDeltaZ = default;
    [SerializeField] private float liftDuration = default;
    [SerializeField] private float fadeDuration = default;
    [SerializeField] private Ease inEase = default;
    [SerializeField] private Ease outEase = default;

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