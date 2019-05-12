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
    public class Factory : PlaceholderFactory<ISeaView>
    {
    }
    
    private static readonly int Curvature = Shader.PropertyToID("_Curvature");
    private static readonly int PivotOffset = Shader.PropertyToID("_PivotOffset");
    
    [SerializeField] private SlotView[] slots;
    [SerializeField] private Transform slotWrapper;
    [SerializeField] private MeshRenderer background;
    [SerializeField] private float height;

    [Inject] private Viewport viewport;
    private Material oceanMaterial;
    private Sequence projectionSequence;

    public float Height => height;
    public ISlotView[] Slots => slots;

    private void Awake()
    {
        var viewportHeight = viewport.Size.y;
        var backgroundTransform = background.transform;

        backgroundTransform.localScale = new Vector3(viewportHeight * 1.25f, viewportHeight * 1.25f, 1f);
        
        oceanMaterial = background.sharedMaterial;
        oceanMaterial.SetFloat(PivotOffset, - (Height / viewportHeight) * 0.5f);
        oceanMaterial.SetFloat(Curvature, 1);
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