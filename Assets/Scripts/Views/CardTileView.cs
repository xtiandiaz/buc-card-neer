using UniRx;
using UnityEngine;
using Zenject;

public class CardTileView : MonoBehaviour
{
    public class Factory : PlaceholderFactory<ICardTile, CardTileView>
    {
    }

    [SerializeField] private SpriteRenderer forbiddenIconRenderer;
    private ICardTile cardTile;
    private GameSettings settings;

    [Inject]
    private void Construct(ICardTile cardTile, GameSettings settings)
    {
        this.cardTile = cardTile;
        this.settings = settings;
    }

    private void Awake()
    {
        var cardCoords = cardTile.Coordinates;
        transform.localPosition = new Vector3(cardCoords.x, cardCoords.y, 0) * settings.DisplacementUnit;

        cardTile.IsLockedAsObservable.Subscribe(isLocked => forbiddenIconRenderer.enabled = isLocked)
            .AddTo(this);
    }
}