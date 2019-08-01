using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

interface IMainMenu : IMenu
{
}

public class MainMenu : Menu, IMainMenu
{
    [SerializeField] private TextButton playButton = default;

    [Inject] private IAppNavigator appNavigator = default; 

    private void Start()
    {
        playButton.OnPointerClickAsObservable()
            .Take(1)
            .Subscribe(_ => appNavigator.GoToGame())
            .AddTo(this);
    }
}