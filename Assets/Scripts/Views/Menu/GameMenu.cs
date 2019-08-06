using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public interface IGameMenuView : IMenu
{
}

public class GameMenu : WorldSpaceMenu, IGameMenuView
{
    [SerializeField] private GameObject contentWrapper = default;
    [SerializeField] private Text heading = default;

    [Header("Labels")] 
    [SerializeField] private Text undealtCount = default;
    
    [Header("Controls")]
    [SerializeField] private ButtonIcon pauseButton = default;

    private IGameStatus gameStatus;
    private IMenuFactory menuFactory;
    private IAudioManager audioManager;

    [Inject]
    private void Initialize(
        IGameStatus gameStatus,
        IMenuFactory menuFactory,
        IAudioManager audioManager
        )
    {
        this.gameStatus = gameStatus;
        this.menuFactory = menuFactory;
        this.audioManager = audioManager;
    }

    private void Awake()
    {
        contentWrapper.SetActive(false);
    }

    private void Start()
    {
        gameStatus.WhenLost
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ =>
            {
                ShowHeadline("Game Over", Color.red);
                
                audioManager.Play(AudioEventKey.GameLose);
            })
            .AddTo(this);

        gameStatus.WhenWon
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(score =>
            {
                ShowHeadline(
                    $"You won!\n<size=10>Earned <color=FFFFFF>{score}</color> coins.</size>",
                    Color.yellow);
                
                audioManager.Play(AudioEventKey.GameLose);
            })
            .AddTo(this);

        gameStatus.UndealtCardCount
            .SubscribeToText(undealtCount)
            .AddTo(this);

        pauseButton.WhenClicked
            .Do(_ => pauseButton.gameObject.SetActive(false))
            .Select(_ => menuFactory.Create<IPauseMenu>())
            .SelectMany(pauseMenu => pauseMenu.WhenClosed)
            .Do(_ => pauseButton.gameObject.SetActive(true))
            .Subscribe()
            .AddTo(this);
    }

    private void ShowHeadline(string withText, Color andColor)
    {
        heading.text = withText;
        heading.color = andColor;
        contentWrapper.SetActive(true);
    }
}