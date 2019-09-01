using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public interface ICardView
{
    int Value { set; }
    int LockValue { set; }
    string Name { set; }
    string ObjectName { get; }
    CardFace Face { set; }
    ISuitModel Suit { set; }
    
    bool IsBoarded { set; }
    
    Sprite FrontCover { set; }
    Sprite BackCover { set; }
    Sprite FrontMotif { set; }
    Sprite BackMotif { set; }
    
    Vector3 Position { get; set; }
    Vector3 LocalPosition { get; }

    void Pick(Vector3 atPosition);
    void Drag(Vector3 toPosition);
    void Sort(int withRawIndex);
    void ToggleValueVisibility(bool toValue);
    void ToggleLockVisibility(bool toValue);
    void Destroy();
    
    Sequence Arrange(ArrangementInfo withInfo);
    Sequence Reveal();
    Sequence Lodge(LodgingInfo withInfo);
    Sequence Fling(Vector3 toPosition, Ease withEase, float andDuration);
    Tween Fade(float toAlphaValue, float withDuration);
    void Bounce(Vector3 withVector);    
    
    IObservable<Unit> Clash(Direction toward);
    IObservable<Unit> OnClashed();
    IObservable<Unit> OnShot();
}

public class CardView : MonoBehaviour, ICardView
{
    private readonly ReactiveProperty<bool> isBoarded = new ReactiveProperty<bool>(); 
        
    [SerializeField] protected CardCustomizer customizer = default;
    [SerializeField] private CardAnimator animator = default;
    [SerializeField] private CardSorter sorter = default;
    [SerializeField] private CardShader shader = default;

    [SerializeField] private Transform floatingWrapper = default;

    private bool isPicked;
    private float floatingT;
    private int? lastParentIndex;
    private Tween dragging;
    private Sequence arranging;

    public virtual int Value
    {
        set => customizer.Value = value;
    }

    public int LockValue
    {
        set => customizer.LockValue = value;
    }

    public string Name
    {
        set => customizer.CardName = value;
    }

    public string ObjectName => name;

    public virtual ISuitModel Suit
    {
        set => customizer.Suit = value;
    }

    public bool IsBoarded
    {
        private get => isBoarded.Value;
        set => isBoarded.Value = value;
    }

    public Sprite FrontCover
    {
        set => customizer.FrontCover = value;
    }

    public Sprite BackCover
    {
        set => customizer.BackCover = value;
    }
    
    public Sprite FrontMotif
    {
        set => customizer.FrontMotif = value;
    }

    public Sprite BackMotif
    {
        set => customizer.BackMotif = value;
    }
    
    public CardFace Face
    {
        set => customizer.Face = value;
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
    
    public Vector3 LocalPosition => transform.localPosition;

    private void Start()
    {
        Observable.EveryGameObjectUpdate()
            .TakeUntil(isBoarded.Where(value => value))
            .Do(_ => floatingT -= Time.deltaTime * 2f)
            .Where(_ => !isPicked)
            .Subscribe(_ =>
            {
                floatingWrapper.localPosition = 0.05f * Mathf.Sin(floatingT) * Vector3.up;
            })
            .AddTo(this);
    }

    public void Pick(Vector3 atPosition)
    {
        sorter.Order = 100;
        isPicked = true;

        arranging?.Kill();

        Drag(atPosition);
    }

    public void Drag(Vector3 toPosition)
    {
        dragging?.Kill();
        dragging = transform.DOMove(toPosition, 0.1f)
            .SetEase(Ease.OutQuart);
    }

    public Sequence Lodge(LodgingInfo withInfo)
    {
        dragging?.Kill();
            
        transform.SetParent(withInfo.Bond.Transform, true);

        Sort(withInfo.ArrangementInfo.Index);

        if (!IsBoarded && lastParentIndex != withInfo.Bond.Index)
        {
            lastParentIndex = withInfo.Bond.Index;
            floatingT = lastParentIndex.Value * Mathf.PI * 0.5f;
        }
            
        isPicked = false;

        return animator.Arrange(withInfo.ArrangementInfo);
    }

    public Sequence Arrange(ArrangementInfo withInfo)
    {
        dragging?.Kill();
        arranging?.Kill();

        isPicked = false;
        arranging = animator.Arrange(withInfo);

        return arranging;
    }

    public Sequence Reveal()
    {
        return animator.Flip(CardFace.Front);
    }

    public IObservable<Unit> Clash(Direction toward)
    {
        return animator.Clash(toward)
            .DoOnSubscribe(() => sorter.Order += 1)
            .DoOnCompleted(() => sorter.Order -= 1);
    }

    public IObservable<Unit> OnClashed()
    {
        return animator.OnClashed();
    }
    
    public IObservable<Unit> OnShot()
    {
        return animator.OnShot();
    }

    public Sequence Fling(Vector3 toPosition, Ease withEase, float andDuration)
    {
        return animator.Fling(toPosition, withEase, andDuration);
    }

    public void Bounce(Vector3 withVector)
    {
        animator.Bounce(withVector, 0.5f);
    }

    public Tween Fade(float toAlphaValue, float withDuration)
    {
        return shader.Fade(toAlphaValue, withDuration);
    }

    public void ToggleValueVisibility(bool toValue)
    {
        customizer.ToggleValueVisibility(toValue);
    }
    
    public void ToggleLockVisibility(bool toValue)
    {
        customizer.ToggleLockVisibility(toValue);
    }

    public void Sort(int withRawIndex)
    {
        sorter.Order = -withRawIndex;
    }
    
    public void Destroy()
    {
        Destroy(gameObject);
    }
}