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

    private IUserSettings userSettings;
    private ILocalizator localizator;
    private IMenuFactory menuFactory;

    [Inject]
    private void Initialize(
        IUserSettings userSettings,
        ILocalizator localizator,
        IMenuFactory menuFactory
        )
    {
        audioToggle.SetState(userSettings.ShouldPlayAudio);
        deviceCardToggle.SetState(userSettings.ShouldDealDeviceCards);

        this.userSettings = userSettings;
        this.localizator = localizator;
        this.menuFactory = menuFactory;
    }

    protected override void Start()
    {
        base.Start();
        
        localizator.Hook(audioToggle, "ui.toggle.audio");
        localizator.Hook(deviceCardToggle, "ui.toggle.deviceCards");
        
        localizator.Hook(languageButton, "ui.button.language");

        audioToggle.WhenStateChanged
            .Subscribe(value => userSettings.ShouldPlayAudio = value)
            .AddTo(this);
        
        deviceCardToggle.WhenStateChanged
            .Subscribe(value => userSettings.ShouldDealDeviceCards = value)
            .AddTo(this);

        languageButton.WhenClicked
            .Subscribe(_ => menuFactory.Create<ILanguageSelectionMenu>())
            .AddTo(this);
    }
}