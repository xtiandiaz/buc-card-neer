using UniRx;
using UnityEngine;
using Zenject;

public class GameController : IInitializable
{
    private readonly BoardController boardController;
    private readonly GameSettings settings;

    public GameController(
        BoardController boardController, 
        GameSettings settings
    )
    {
        this.boardController = boardController;
        this.settings = settings;
    }

    public void Initialize()
    {
        Application.targetFrameRate = 50;
        
        boardController.PlayerCard.ObservableHealth
            .Where(h => h == 0)
            .Delay(settings.MoveDuration)
            .Subscribe(_ =>
            {
                boardController.PlayerCardView.Flip();
                boardController.Dispose();
            });
    }
}