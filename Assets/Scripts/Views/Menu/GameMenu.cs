using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public interface IGameMenuView : IMenu
{
}

public class GameMenu : Menu, IGameMenuView
{
    [SerializeField] private GameObject contentWrapper = default;
    [SerializeField] private Text heading = default;

    [Header("Labels")] 
    [SerializeField] private Text undealtCount = default;
    
    [Header("Controls")]
    [SerializeField] private Button resetButton = default;

    [Inject] private IGameStatus gameStatus = default;

    private void Awake()
    {
        contentWrapper.SetActive(false);
    }

    private void Start()
    {
        gameStatus.WhenLost
            .Subscribe(_ => ShowHeadline("Game Over", Color.red))
            .AddTo(this);

        gameStatus.WhenWon
            .Subscribe(score => ShowHeadline(
                $"You won!\n<size=10>Earned <color=FFFFFF>{score}</color> coins.</size>",
                Color.yellow))
            .AddTo(this);

        gameStatus.UndealtCardCount
            .SubscribeToText(undealtCount)
            .AddTo(this);

        resetButton.OnClickAsObservable()
            .Subscribe(_ => gameStatus.Reset())
            .AddTo(this);
    }

    private void ShowHeadline(string withText, Color andColor)
    {
        heading.text = withText;
        heading.color = andColor;
        contentWrapper.SetActive(true);
    }
}