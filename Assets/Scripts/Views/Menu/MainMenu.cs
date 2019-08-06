using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

interface IMainMenu : IMenu
{
}

public class MainMenu : WorldSpaceMenu, IMainMenu
{
    [SerializeField] private ButtonText playButton = default;
    [SerializeField] private Text buildLabel = default;

    private IAppNavigator appNavigator;
    
    [Inject]
    private void Initialize(IAppNavigator appNavigator)
    {
        this.appNavigator = appNavigator;
    }

    private void Start()
    {
        playButton.WhenClicked
            .Take(1)
            .Subscribe(_ => appNavigator.GoToGame())
            .AddTo(this);
        

        buildLabel.text = $"{Application.version} ({5})";
    }
}