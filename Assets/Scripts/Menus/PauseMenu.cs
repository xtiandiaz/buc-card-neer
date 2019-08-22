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
    [SerializeField] private ToggleText audioToggle = default;

    private IGameStatus gameStatus;
    private IAppNavigator appNavigator;
    private IPlayerSettings playerSettings;

    [Inject]
    private void Initialize(
        IGameStatus gameStatus,
        IAppNavigator appNavigator,
        IPlayerSettings playerSettings
        )
    {
        audioToggle.SetState(playerSettings.ShouldPlayAudio);
        
        this.gameStatus = gameStatus;
        this.appNavigator = appNavigator;
        this.playerSettings = playerSettings;
    }

    protected override void Start()
    {
        base.Start();
        
        restartButton.WhenClicked
            .Take(1)
            .Subscribe(_ => gameStatus.Reset())
            .AddTo(this);
        
        quitButton.WhenClicked
            .Take(1)
            .Subscribe(_ => appNavigator.GoToMainMenu())
            .AddTo(this);
        
        audioToggle.WhenStateChanged
            .Subscribe(value => playerSettings.ShouldPlayAudio = value)
            .AddTo(this);
    }
}