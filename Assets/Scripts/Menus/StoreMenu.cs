using UnityEngine;
using UnityEngine.UI;

public interface IStoreMenu : IMenu
{
}

public class StoreMenu : WorldSpaceMenu, IStoreMenu
{
    [SerializeField] private Text aboutInfo = default;

    protected override void Start()
    {
        base.Start();
        
        localizator.Hook(aboutInfo, "ui.info.aboutStore");
    }
}