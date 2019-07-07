using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public interface IGameMenuView : IInitializable
{
}

public class GameMenuView : MenuView, IGameMenuView
{
    [SerializeField] private GameObject contentWrapper;
    [SerializeField] private Button resetButton;
    [SerializeField] private Text heading;
    
    [Inject] private IGameController gameController;

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