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
    [SerializeField] private Text buildLabel = default;

    private IAppNavigator appNavigator;
    private IAppInfo appInfo;
    
    [Inject]
    private void Initialize(
        IAppNavigator appNavigator,
        IAppInfo appInfo
        )
    {
        this.appNavigator = appNavigator;
        this.appInfo = appInfo;
    }

    private void Start()
    {
        playButton.WhenClicked
            .Take(1)
            .Subscribe(_ => appNavigator.GoToGame())
            .AddTo(this);
        

        buildLabel.text = $"{Application.version} ({appInfo.BuildNumber})";
    }
}