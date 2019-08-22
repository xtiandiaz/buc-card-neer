using UniRx;
using UnityEngine;
using Zenject;

public interface ISettingsMenu : IMenu
{
}

public class SettingsMenu : WorldSpaceMenu, ISettingsMenu
{
    [SerializeField] private ToggleText audioToggle = default;
    [SerializeField] private ToggleText deviceCardToggle = default;
    [SerializeField] private ButtonText languageButton = default;
    [SerializeField] private ButtonText clearStatsButton = default;

    private IPlayerSettings playerSettings;
    private IMenuFactory menuFactory;
    private IPlayerStats playerStats;

    [Inject]
    private void Initialize(
        IPlayerSettings playerSettings,
        IPlayerStats playerStats,
        IMenuFactory menuFactory
        )
    {
        audioToggle.SetState(playerSettings.ShouldPlayAudio);
        deviceCardToggle.SetState(playerSettings.ShouldDealDeviceCards);

        this.playerSettings = playerSettings;
        this.playerStats = playerStats;
        this.menuFactory = menuFactory;
    }

    protected override void Start()
    {
        base.Start();
        
        localizator.Hook(audioToggle, "ui.toggle.audio");
        localizator.Hook(deviceCardToggle, "ui.toggle.deviceCards");
        
        localizator.Hook(languageButton, "ui.button.language");
        localizator.Hook(clearStatsButton, "ui.button.clearPlayerStats");

        audioToggle.WhenStateChanged
            .Subscribe(value => playerSettings.ShouldPlayAudio = value)
            .AddTo(this);
        
        deviceCardToggle.WhenStateChanged
            .Subscribe(value => playerSettings.ShouldDealDeviceCards = value)
            .AddTo(this);

        languageButton.WhenClicked
            .Subscribe(_ => menuFactory.Create<ILanguageSelectionMenu>())
            .AddTo(this);
        
        clearStatsButton.WhenClicked
            .Subscribe(_ =>
            {
                playerStats.Clear();
                Close();
            })
            .AddTo(this);
    }
}