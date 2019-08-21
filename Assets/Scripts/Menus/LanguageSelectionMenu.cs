using UniRx;
using UnityEngine;
using Zenject;

public interface ILanguageSelectionMenu : IMenu
{
}

public class LanguageSelectionMenu : WorldSpaceMenu, ILanguageSelectionMenu
{
    [SerializeField] private ToggleText sampleToggle = default;
    [SerializeField] private Transform controlsWrapper = default;
    
    [Inject]
    private void Initialize(
        IPlayerSettings playerSettings,
        ILocalizationManager localizationManager
        )
    {
        foreach (var language in localizationManager.GetSupportedLanguages())
        {
            var toggle = Instantiate(sampleToggle, controlsWrapper, false);
            
            toggle.SetText(language.Name);
            toggle.SetState(playerSettings.Language == language.Key);

            toggle.WhenClicked
                .Select(_ => language.Key)
                .Subscribe(lang =>
                {
                    playerSettings.Language = lang;
                    Close();
                })
                .AddTo(this);
        }
        
        sampleToggle.gameObject.SetActive(false);
    }
}