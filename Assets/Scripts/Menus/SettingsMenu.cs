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

    private IPlayerSettings playerSettings;
    private IMenuFactory menuFactory;

    [Inject]
    private void Initialize(
        IPlayerSettings playerSettings,
        IMenuFactory menuFactory
        )
    {
        audioToggle.SetState(playerSettings.ShouldPlayAudio);
        deviceCardToggle.SetState(playerSettings.ShouldDealDeviceCards);

        this.playerSettings = playerSettings;
        this.menuFactory = menuFactory;
    }

    protected override void Start()
    {
        base.Start();
        
        localizator.Hook(audioToggle, "ui.toggle.audio");
        localizator.Hook(deviceCardToggle, "ui.toggle.deviceCards");
        
        localizator.Hook(languageButton, "ui.button.language");

        audioToggle.WhenStateChanged
            .Subscribe(value => playerSettings.ShouldPlayAudio = value)
            .AddTo(this);
        
        deviceCardToggle.WhenStateChanged
            .Subscribe(value => playerSettings.ShouldDealDeviceCards = value)
            .AddTo(this);

        languageButton.WhenClicked
            .Subscribe(_ => menuFactory.Create<ILanguageSelectionMenu>())
            .AddTo(this);
    }
}