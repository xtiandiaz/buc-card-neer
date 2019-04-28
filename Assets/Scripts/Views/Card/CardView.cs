using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public interface ICardView
{
    SpriteRenderer FrontFaceRenderer { get; }
    SpriteRenderer BackFaceRenderer { get; }
    Vector3 Position { get; set; }
    
    void OnPicked();
    void OnDragged(Vector3 deltaPosition);
    void OnDropped();
    void Set(Vector3 positionAnimated, TimeSpan during);
    void Destroy();
    T AddComponent<T>() where T : Component;
}

public class CardView : MonoBehaviour, ICardView
{
    public class Factory : PlaceholderFactory<string, CardView>
    {
    }

    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private SpriteRenderer frontFaceRenderer;
    [SerializeField] private SpriteRenderer backFaceRenderer;
    
    private GameSettings settings;
    private MeshRenderer textMeshRenderer;
    private Transform thisTransform;
    private Tween positionTween;

    public SpriteRenderer FrontFaceRenderer => frontFaceRenderer;
    public SpriteRenderer BackFaceRenderer => backFaceRenderer;

    public Vector3 Position
    {
        get => thisTransform.position;
        set
        {
            positionTween?.Kill();
            thisTransform.position = value;
        }
    }

    [Inject]
    private void Construct(GameSettings settings)
    {
        this.settings = settings;
        
        thisTransform = transform;

        sortingGroup.enabled = false;
    }

    public void OnPicked()
    {
        sortingGroup.enabled = true;
        sortingGroup.sortingOrder = settings.FloatingCardSortingOrder;

        positionTween?.Kill();
    }

    public void OnDragged(Vector3 deltaPosition)
    {
        thisTransform.localPosition += deltaPosition;
    }

    public void OnDropped()
    {
        sortingGroup.enabled = false;
    }

    public void Set(Vector3 position)
    {
        positionTween?.Kill();
        
        thisTransform.position = position;
    }

    public void Set(Vector3 positionAnimated, TimeSpan during)
    {
        Move(positionAnimated, during);
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
    }

    public T AddComponent<T>() where T : Component
    {
        return gameObject.AddComponent<T>();
    }

    private void Move(Vector3 toPosition, TimeSpan during, TweenCallback andDoOncomplete = null)
    {
        positionTween?.Kill();
        positionTween = thisTransform.DOLocalMove(toPosition, (float) during.TotalSeconds)
            .SetEase(Ease.OutQuint);

        if (andDoOncomplete != null)
            positionTween.OnComplete(andDoOncomplete);
    }
}