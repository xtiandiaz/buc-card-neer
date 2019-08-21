using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IMenu
{
    IObservable<Unit> WhenClosed { get; }

    void Close();
}

[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public abstract class Menu : MonoBehaviour, IMenu
{
    private readonly Subject<Unit> closing = new Subject<Unit>();

    protected ILocalizator localizator;
    
    [SerializeField] protected Transform contentWrapper = default;
    [SerializeField] private CustomButton closeButton = default;

    public IObservable<Unit> WhenClosed => closing;

    [Inject]
    private void Initialize(
        ILocalizator localizator
    )
    {
        this.localizator = localizator;
    }
    
    protected virtual void Start()
    {
        if (closeButton != null)
        {
            closeButton.WhenClicked
                .Take(1)
                .Subscribe(_ => Close())
                .AddTo(this);
        }
    }

    public void Close()
    {
        closing.OnNext(Unit.Default);
        closing.OnCompleted();
        
        Destroy(gameObject);
    }
}