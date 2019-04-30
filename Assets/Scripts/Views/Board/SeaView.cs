using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;

public interface ISeaView
{
    float Height { get; }
    ISlotView[] Slots { get; }

    void ToggleProjection(bool on, float withDurationInSeconds, float andDelayInSeconds = 0);
}

public class SeaView : MonoBehaviour, ISeaView
{
    private static readonly int Curvature = Shader.PropertyToID("_Curvature");
    private static readonly int PivotOffset = Shader.PropertyToID("_PivotOffset");
    
    [SerializeField] private SlotView[] slots;
    [SerializeField] private Transform slotWrapper;
    [SerializeField] private MeshRenderer background;
    
    private GameSettings settings;
    private Material oceanMaterial;
    private Sequence projectionSequence;

    public float Height => settings.CardSize.y;
    public ISlotView[] Slots => slots;

    [Inject]
    private void Construct(
        GameSettings settings
    )
    {
        this.settings = settings;
    }

    public void Initialize(float withBoardHeight)
    {
        var backgroundTransform = background.transform;
        
        backgroundTransform.localScale = new Vector3(withBoardHeight * 1.25f, withBoardHeight * 1.25f, 1f);
        //backgroundTransform.localPosition = Vector3.forward * settings.CardSize.x;
        
        oceanMaterial = background.sharedMaterial;
        oceanMaterial.SetFloat(PivotOffset, - (Height / withBoardHeight) * 0.5f);
    }

    public void ToggleProjection(bool on, float withDurationInSeconds, float andDelayInSeconds = 0)
    {
        projectionSequence?.Kill();
        projectionSequence = DOTween.Sequence();
        
        projectionSequence.Join(
            DOTween.To(
                () => oceanMaterial.GetFloat(Curvature),
                c => oceanMaterial.SetFloat(Curvature, c),
                on ? 1f : 0,
                withDurationInSeconds));

        projectionSequence.SetDelay(andDelayInSeconds);
        projectionSequence.SetEase(Ease.InOutSine);
        
        if (on)
            projectionSequence.OnComplete(() => slotWrapper.gameObject.SetActive(true));
        else
            projectionSequence.OnStart(() => slotWrapper.gameObject.SetActive(false));
    }
}