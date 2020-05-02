using System;
using UniRx;
using UnityEngine;
using Zenject;

public interface IVisualEffectsController : IInitializable, IDisposable
{
}

public class VisualEffectsController : IVisualEffectsController
{
    private readonly IGameCamera camera;
    private readonly IGameStatus gameStatus;
    private readonly IConfrontationController confrontator;
    private readonly IBoard board;
    private readonly IShip ship;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private VisualEffectsController(
        IGameCamera camera,
        IGameStatus gameStatus,
        IConfrontationController confrontator,
        IBoard board
    )
    {
        this.camera = camera;
        this.gameStatus = gameStatus;
        this.confrontator = confrontator;
        this.board = board;
    }

    public void Initialize()
    {
        disposables.Add(gameStatus.WhenPlayerShot
                .Subscribe(_ => camera.Shake(0.75f, TimeSpan.FromSeconds(0.5))));
        
        disposables.Add(board.Ship.WhenCardBoarded
            .Where(type => (type & CardType.Monster) != 0)
            .Subscribe(_ => camera.Shake(0.25f, TimeSpan.FromSeconds(1), 4)));
        
        disposables.Add(gameStatus.WhenPlayerAttackedOnBoard
            .Merge(confrontator.WhenPlayerConfronted.AsUnitObservable())
            .Subscribe(_ => camera.Shake(0.15f, TimeSpan.FromSeconds(0.5), 2)));
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }

    public static void ToggleTintFilterToSupplySlots()
    {
        GameObject[] SupplySlotsInScene = GameObject.FindGameObjectsWithTag("SupplyCardFilter");

        for (int i = 0; i < SupplySlotsInScene.Length; i++)
        {
            Debug.Log("Supply Slots: " + i);
            var supplySlotFilters = SupplySlotsInScene[i].GetComponent<SpriteRenderer>();
            supplySlotFilters.enabled = !supplySlotFilters.enabled;
        }
    }
}