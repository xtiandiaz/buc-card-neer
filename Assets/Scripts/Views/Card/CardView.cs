using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public interface ICardView
{
    SpriteRenderer FrontFaceRenderer { get; }
    SpriteRenderer BackFaceRenderer { get; }
    Transform Transform { get; }
    
    void Destroy();
    void Arrange(Vector3 atLocalPos, int andIndexInStack, int withStackCount, CardStackLayout andLayout);
    void OnBeginDrag();
    void OnDrag(Vector3 deltaPosition);
    void OnDrop();
}

public class CardView : MonoBehaviour, ICardView
{
    public class Factory : PlaceholderFactory<string, CardView>
    {
        public CardView Create(CardType withType)
        {
            return base.Create($"Prefabs/Card{withType.ToString()}");
        }
    }

    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private SpriteRenderer frontFaceRenderer;
    [SerializeField] private SpriteRenderer backFaceRenderer;
    
    private GameSettings settings;
    private MeshRenderer textMeshRenderer;
    private Tween locationTween;
    private int defaultSortingOrder;
    private Vector3 defaultLocalPosition;
    private bool isFirstArrangement = true;

    public SpriteRenderer FrontFaceRenderer => frontFaceRenderer;
    public SpriteRenderer BackFaceRenderer => backFaceRenderer;
    public Transform Transform { get; private set; }
    
    [Inject]
    private void Construct(GameSettings settings)
    {
        this.settings = settings;
        
        Transform = transform;

        sortingGroup.enabled = false;
    }

    public void Arrange(Vector3 atLocalPos, int andIndexInStack, int withStackCount, CardStackLayout andLayout)
    {
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
        sortingGroup.enabled = true;
        sortingGroup.sortingOrder = settings.FloatingCardSortingOrder;

        locationTween?.Kill();

        // TODO Animate lift
        Transform.localPosition += Vector3.back;
    }

    public void OnDrag(Vector3 deltaPosition)
    {
        Transform.localPosition += deltaPosition;
    }

    public void OnDrop()
    {
        sortingGroup.enabled = false;
        
        Move(
            defaultLocalPosition, 
            settings.CardReturnDuration,
            () => sortingGroup.enabled = false);
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