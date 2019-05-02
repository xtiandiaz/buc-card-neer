using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipAnimationSettings", menuName = "Game/Settings/Ship Animation Settings", order = 1)]
public class ShipAnimationSettings : ScriptableObject
{
    [SerializeField] private float dockingDuration = 1f;
    [SerializeField] private float dockingDelay = 0;
    [SerializeField] private float sailingDuration = 1f;
    [SerializeField] private Ease dockingEase = Ease.OutQuart;
    [SerializeField] private Ease sailingEase = Ease.InQuart;
    
    public float DockingDuration => dockingDuration;
    public float SailingDuration => sailingDuration;
    public float DockingDelay => dockingDelay;
    public Ease DockingEase => dockingEase;
    public Ease SailingEase => sailingEase;
}