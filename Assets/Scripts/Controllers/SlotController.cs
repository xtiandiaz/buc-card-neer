using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface ISlotController
{
    SlotType Type { get; }
    uint Capacity { get; }

    void Initialize();
}

public class SlotController : ISlotController, IDisposable
{
    public class Factory : PlaceholderFactory<ISlot, ISlotView, SlotController>
    {
    }
    
    private readonly ISlot model;
    private readonly ISlotView view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private SlotController(
        ISlot model,
        ISlotView view
        )
    {
        this.model = model;
        this.view = view;
    }
    
    public SlotType Type => model.Type;
    public uint Capacity => model.Capacity;

    public void Initialize()
    {
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}