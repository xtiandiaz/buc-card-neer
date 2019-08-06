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

    [Inject]
    private void Initialize(
        IGameStatus gameStatus,
        IMenuFactory menuFactory
        )
    {
        this.gameStatus = gameStatus;
        this.menuFactory = menuFactory;
    }

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