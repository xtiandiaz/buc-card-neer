using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public interface IGameMenu : IMenu
{
}

public class GameMenu : WorldSpaceMenu, IGameMenu
{
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
        contentWrapper.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();

        gameStatus.UndealtCardCount
            .SubscribeToText(undealtCount)
            .AddTo(this);

        pauseButton.WhenClicked
            .Select(_ => menuFactory.Create<IPauseMenu>())
            .SelectMany(pauseMenu => pauseMenu.WhenClosed)
            .Subscribe()
            .AddTo(this);
    }
}