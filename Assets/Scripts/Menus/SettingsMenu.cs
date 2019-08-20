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

    private IUserSettings userSettings;
    
    [Inject]
    private void Initialize(
        IUserSettings userSettings
        )
    {
        audioToggle.SetState(userSettings.ShouldPlayAudio);
        deviceCardToggle.SetState(userSettings.ShouldDealDeviceCards);

        this.userSettings = userSettings;
    }

    protected override void Start()
    {
        base.Start();

        audioToggle.WhenStateChanged
            .Subscribe(value => userSettings.ShouldPlayAudio = value)
            .AddTo(this);
        
        deviceCardToggle.WhenStateChanged
            .Subscribe(value => userSettings.ShouldDealDeviceCards = value)
            .AddTo(this);
    }
}