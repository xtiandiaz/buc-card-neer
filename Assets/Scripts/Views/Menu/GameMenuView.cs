using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public interface IGameMenuView : IInitializable
{
}

public class GameMenuView : MenuView, IGameMenuView
{
    [SerializeField] private GameObject contentWrapper = default;
    [SerializeField] private Button resetButton = default;
    [SerializeField] private Text heading = default;
    
    [Inject] private IGameController gameController = default;

    [Inject]
    public void Initialize()
    {
        contentWrapper.SetActive(false);
        
        gameController.WhenLost
            .Subscribe(_ =>
            {
                heading.text = "Game Over";
                contentWrapper.SetActive(true);
            })
            .AddTo(this);

        resetButton.OnClickAsObservable()
            .Subscribe(_ => gameController.Reset())
            .AddTo(this);
    }
}