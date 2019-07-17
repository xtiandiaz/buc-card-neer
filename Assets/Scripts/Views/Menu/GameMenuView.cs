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
    [SerializeField] private Text heading = default;

    [Header("Labels")] 
    [SerializeField] private Text undealtCount = default;
    
    [Header("Controls")]
    [SerializeField] private Button resetButton = default;
    
    [Inject] private IGameStatus gameStatus = default;
    
    public void Initialize()
    {
        contentWrapper.SetActive(false);
        
        gameStatus.WhenLost
            .Subscribe(_ =>
            {
                heading.text = "Game Over";
                contentWrapper.SetActive(true);
            })
            .AddTo(this);

        gameStatus.UndealtCardCount
            .SubscribeToText(undealtCount)
            .AddTo(this);

        resetButton.OnClickAsObservable()
            .Subscribe(_ => gameStatus.Reset())
            .AddTo(this);
    }
}