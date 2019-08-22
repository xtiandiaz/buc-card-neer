using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IStoreMenu : IMenu
{
}

public class StoreMenu : WorldSpaceMenu, IStoreMenu
{
    [SerializeField] private Text balanceField = default;

    [Inject]
    private void Initialize(
        IPlayerStats playerStats
        )
    {
        balanceField.text = $"{playerStats.Balance}";
    }
}