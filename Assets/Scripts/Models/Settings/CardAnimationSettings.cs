using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAnimationSettings", menuName = "Game/Settings/Card Animation Settings", order = 1)]
public class CardAnimationSettings : ScriptableObject
{
    [field: SerializeField] public float MoveDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float FlipDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float LiftDepth { get; private set; } = 2.5f;
    [field: SerializeField] public float LiftDuration { get; private set; } = 0.2f;
    [field: SerializeField] public float ReturnToLocationWerePickedDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float BoardingDelay { get; private set; }
    [field: SerializeField] public Ease InEase { get; private set; } = Ease.InQuart;
    [field: SerializeField] public Ease OutEase { get; private set; } = Ease.OutQuart;
}