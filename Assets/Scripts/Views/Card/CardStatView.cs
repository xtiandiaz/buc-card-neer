using System;
using UniRx;
using UnityEngine;

public class CardStatView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconRenderer;
    [SerializeField] private TextMesh textMesh;

    private IDisposable valueSubscription;
    
    public void Observe(IObservable<int> observable)
    {
        valueSubscription?.Dispose();
        valueSubscription = observable.Subscribe(value => textMesh.text = $"{value}");
    }

    private void OnDestroy()
    {
        valueSubscription?.Dispose();
    }
}