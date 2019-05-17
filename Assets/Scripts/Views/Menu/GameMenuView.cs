using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IGameMenuView
{
    Button ResetControl { get; }
}

public class GameMenuView : MenuView, IGameMenuView
{
    [SerializeField] private Button resetButton;
    [SerializeField] private Text heading;

    public Button ResetControl => resetButton;

    [Inject]
    private void Initialize(IGameStatusNotifier gameStatusNotifier)
    {
        gameStatusNotifier.WhenEnded
            .Subscribe(_ => heading.text = "Game Over")
            .AddTo(this);
    }
}