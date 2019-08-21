using UnityEngine;
using UnityEngine.UI;

public interface ILogbookMenu : IMenu
{
}

public class LogbookMenu : WorldSpaceMenu, ILogbookMenu
{
    [SerializeField] private Text aboutInfo = default;

    protected override void Start()
    {
        base.Start();
        
        localizator.Hook(aboutInfo, "ui.info.aboutLogbook");
    }
}