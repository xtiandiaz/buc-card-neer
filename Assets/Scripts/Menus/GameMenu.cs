using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public interface IGameMenu : IMenu
{
}

public class GameMenu : WorldSpaceMenu, IGameMenu
{
    [SerializeField] private Text heading = default;

    [Header("Labels")] 
    [SerializeField] private Text undealtCount = default;
    
    [Header("Controls")]
    [SerializeField] private ButtonIcon pauseButton = default;

    private IGameStatus gameStatus;
    private IMenuFactory menuFactory;
    private IAudioManager audioManager;
    private ILocalizator localizator;

    [Inject]
    private void Initialize(
        IGameStatus gameStatus,
        IMenuFactory menuFactory,
        IAudioManager audioManager,
        ILocalizator localizator
        )
    {
        this.gameStatus = gameStatus;
        this.menuFactory = menuFactory;
        this.audioManager = audioManager;
        this.localizator = localizator;
    }

    private void Awake()
    {
        contentWrapper.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        
        gameStatus.WhenLost
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ =>
            {
                ShowHeadline(localizator.GetText("ui.headline.gameOver"), Color.red);
                
                audioManager.Play(AudioEventKey.GameLose);
            })
            .AddTo(this);

        gameStatus.WhenWon
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(score =>
            {
                ShowHeadline(
                    localizator.GetText("ui.headline.gameFinished", score),
                    Color.yellow);
                
                audioManager.Play(AudioEventKey.GameWin);
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
        contentWrapper.gameObject.SetActive(true);
    }
}