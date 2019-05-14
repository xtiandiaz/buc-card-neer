using UnityEngine;
using UnityEngine.UI;

public interface IGameMenuView
{
    Button ResetControl { get; }
}

public class GameMenuView : MenuView, IGameMenuView
{
    [SerializeField] private Button resetButton;

    public Button ResetControl => resetButton;
}