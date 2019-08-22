using UniRx;
using UnityEngine;
using Zenject;

public interface IGameOverMenu : IMenu
{
}

public class GameOverMenu : WorldSpaceMenu, IGameOverMenu
{
    [SerializeField] private ButtonText replayButton = default;
    [SerializeField] private ButtonText quitButton = default;

    private IGameStatus gameStatus;
    private IAppNavigator appNavigator;

    [Inject]
    private void Initialize(
        IGameStatus gameStatus,
        IAppNavigator appNavigator
    )
    {
        this.gameStatus = gameStatus;
        this.appNavigator = appNavigator;
    }

    protected override void Start()
    {
        base.Start();

        replayButton.WhenClicked
            .Take(1)
            .Subscribe(_ => gameStatus.Reset())
            .AddTo(this);

        quitButton.WhenClicked
            .Take(1)
            .Subscribe(_ => appNavigator.GoToMainMenu())
            .AddTo(this);
    }
}