using UniRx;
using UnityEngine;
using Zenject;

public interface IPauseMenu : IMenu
{
}

public class PauseMenu : WorldSpaceMenu, IPauseMenu
{
    [SerializeField] private ButtonText restartButton = default;
    [SerializeField] private ButtonText quitButton = default;

    private IGameStatus gameStatus;
    private IAppNavigator appNavigator;
    private ILocalizator localizator;
    
    [Inject]
    private void Initialize(
        IGameStatus gameStatus,
        IAppNavigator appNavigator,
        ILocalizator localizator
        )
    {
        this.gameStatus = gameStatus;
        this.appNavigator = appNavigator;
        this.localizator = localizator;
    }

    protected override void Start()
    {
        base.Start();
        
        localizator.Hook(restartButton, "ui.button.restart");
        localizator.Hook(quitButton, "ui.button.quit");
        
        restartButton.WhenClicked
            .Take(1)
            .Subscribe(_ => gameStatus.Reset())
            .AddTo(this);
        
        quitButton.WhenClicked
            .Take(1)
            .Subscribe(_ => appNavigator.GoToMainMenu())
            .AddTo(this);
    }
}