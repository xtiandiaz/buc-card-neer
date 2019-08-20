using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

interface IMainMenu : IWorldSpaceMenu
{
}

public class MainMenu : WorldSpaceMenu, IMainMenu
{
    [SerializeField] private ButtonText playButton = default;
    [SerializeField] private ButtonIcon settingsButton = default;
    [SerializeField] private Text buildLabel = default;

    private IAppNavigator appNavigator;
    private IMenuFactory menuFactory;
    private IAppInfo appInfo;
    
    [Inject]
    private void Initialize(
        IAppNavigator appNavigator,
        IAppInfo appInfo,
        IMenuFactory menuFactory
        )
    {
        this.appNavigator = appNavigator;
        this.appInfo = appInfo;
        this.menuFactory = menuFactory;
    }

    protected override void Start()
    {
        base.Start();
        
        playButton.WhenClicked
            .Take(1)
            .Subscribe(_ => appNavigator.GoToGame())
            .AddTo(this);

        settingsButton.WhenClicked
            .Do(_ => settingsButton.gameObject.SetActive(false))
            .Select(_ => menuFactory.Create<ISettingsMenu>())
            .SelectMany(menu => menu.WhenClosed)
            .Do(_ => settingsButton.gameObject.SetActive(true))
            .Subscribe()
            .AddTo(this);

        buildLabel.text = $"{Application.version} ({appInfo.BuildNumber})";
    }
}