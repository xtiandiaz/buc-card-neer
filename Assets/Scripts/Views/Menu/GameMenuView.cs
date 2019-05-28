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
    [SerializeField] private GameObject contentWrapper;
    [SerializeField] private Button resetButton;
    [SerializeField] private Text heading;

    public Button ResetControl => resetButton;

    [Inject]
    private void Initialize(IGameStatusNotifier gameStatusNotifier)
    {
        contentWrapper.SetActive(false);
        
        gameStatusNotifier.WhenEnded
            .Subscribe(_ =>
            {
                heading.text = "Game Over";
                contentWrapper.SetActive(true);
            })
            .AddTo(this);
    }
}