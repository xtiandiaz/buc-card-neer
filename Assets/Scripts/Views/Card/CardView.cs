using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public interface ICardView
{
    Transform Transform { get; }
    BoxCollider2D HitArea { get; }
    
    void Destroy();
    void Arrange(Vector3 atLocalPos, int andIndexInStack, int withStackCount, CardStackLayout andLayout);
    void OnBeginDrag();
    void OnDrag(Vector3 deltaPosition);
    void OnDrop();
}

public abstract class CardView : MonoBehaviour, ICardView
{
    public class Factory : IFactory<CardType, CardView>
    {
        private readonly PlayerCardView.Factory playerCardViewFactory;
        private readonly ItemCardView.Factory resourceCardViewFactory;
        private readonly PirateCardView.Factory pirateCardViewFactory;
        private readonly MerchantCardView.Factory merchantCardViewFactory;
        
        private Factory(
            PlayerCardView.Factory playerCardViewFactory,
            ItemCardView.Factory resourceCardViewFactory,
            PirateCardView.Factory pirateCardViewFactory,
            MerchantCardView.Factory merchantCardViewFactory
            )
        {
            this.playerCardViewFactory = playerCardViewFactory;
            this.resourceCardViewFactory = resourceCardViewFactory;
            this.pirateCardViewFactory = pirateCardViewFactory;
            this.merchantCardViewFactory = merchantCardViewFactory;
        }
        
        public CardView Create(CardType withType)
        {
            switch (withType)
            {
                case CardType.Player:
                    return playerCardViewFactory.Create(GetResourceName(withType));
                case CardType.Item:
                    return resourceCardViewFactory.Create(GetResourceName(withType));
                case CardType.Merchant:
                    return merchantCardViewFactory.Create(GetResourceName(withType));
                case CardType.Pirate:
                    return pirateCardViewFactory.Create(GetResourceName(withType));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static string GetResourceName(CardType cardType)
        {
            return $"Prefabs/Cards/{cardType.ToString()}";
        }
    }
    
    [SerializeField] protected SpriteRenderer frontFace;
    [SerializeField] protected SpriteRenderer backFace;
    [SerializeField] protected SortingGroup sortingGroup;
    [SerializeField] private BoxCollider2D hitArea;
    [Header("Text")]
    [SerializeField] private MeshRenderer[] textRenderers;
    [SerializeField] private int textSortingOrder;
    
    private GameSettings settings;
    private MeshRenderer textMeshRenderer;
    private Tween locationTween;
    private int defaultSortingOrder;
    private Vector3 defaultLocalPosition;
    private bool isFirstArrangement = true;

    public Transform Transform { get; private set; }
    public BoxCollider2D HitArea => hitArea;
    
    [Inject]
    private void Construct(
        GameSettings settings
        )
    {
        this.settings = settings;
        
        Transform = transform;
    }

    private void Awake()
    {
        foreach (var textRenderer in textRenderers)
        {
            textRenderer.sortingLayerName = settings.CardSortingLayerName;
            textRenderer.sortingOrder = textSortingOrder;
        }

        Transform.rotation = Quaternion.Euler(0, 180f, 0);
    }

    private void Start()
    {
        Transform.DORotate(Vector3.zero, (float) settings.CardReturnDuration.TotalSeconds)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() =>
            {
                backFace.enabled = false;
            });
    }

    public void Arrange(Vector3 atLocalPos, int andIndexInStack, int withStackCount, CardStackLayout andLayout)
    {
        sortingGroup.sortingOrder = 
            defaultSortingOrder = withStackCount - andIndexInStack - 1;
        
        var positionOffset = andLayout == CardStackLayout.Vertical
            ? Vector3.up * andIndexInStack * settings.CardOffsetInPile.y
            : Vector3.right * defaultSortingOrder * settings.CardOffsetInPile.x;

        defaultLocalPosition = atLocalPos + positionOffset;

        if (isFirstArrangement)
        {
            transform.localPosition = defaultLocalPosition;
            isFirstArrangement = false;
        }
        else
            Move(defaultLocalPosition, settings.CardArrangementDuration);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void OnBeginDrag()
    {
        sortingGroup.sortingOrder = settings.FloatingCardSortingOrder;

        locationTween?.Kill();
    }

    public void OnDrag(Vector3 deltaPosition)
    {
        transform.localPosition += deltaPosition;
    }

    public void OnDrop()
    {
        Move(
            defaultLocalPosition, 
            settings.CardReturnDuration, 
            () => sortingGroup.sortingOrder = defaultSortingOrder);
    }

    private void Move(Vector3 toLocalPosition, TimeSpan during, TweenCallback andDoOncomplete = null)
    {
        locationTween?.Kill();
        locationTween = Transform.DOLocalMove(toLocalPosition, (float) during.TotalSeconds)
            .SetEase(Ease.OutQuint);

        if (andDoOncomplete != null)
            locationTween.OnComplete(andDoOncomplete);
    }
}
