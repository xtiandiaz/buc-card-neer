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
    [SerializeField] private ButtonText storeButton = default;
    [SerializeField] private ButtonText logbookButton = default;
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
        
        storeButton.WhenClicked
            .SelectMany(_ => menuFactory.Create<IStoreMenu>().WhenClosed)
            .Subscribe()
            .AddTo(this);
        
        logbookButton.WhenClicked
            .SelectMany(_ => menuFactory.Create<ILogbookMenu>().WhenClosed)
            .Subscribe()
            .AddTo(this);

        settingsButton.WhenClicked
            .SelectMany(_ => menuFactory.Create<ISettingsMenu>().WhenClosed)
            .Subscribe()
            .AddTo(this);

        buildLabel.text = $"{Application.version} ({appInfo.BuildNumber})";
    }
}