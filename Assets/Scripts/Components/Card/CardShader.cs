using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public interface ICardShader
{
    float Alpha { set; }

    IObservable<Unit> Fade(float toAlphaValue, TimeSpan withDuration);
    void Fog(Color withColor, float byFactor);
}

public class CardShader : MonoBehaviour, ICardShader
{
    private static readonly int FogColorPropertyId = Shader.PropertyToID("_FogColor");
    private static readonly int FogIntensityPropertyId = Shader.PropertyToID("_FogIntensity");

    private readonly List<ShadingEntry> shadingEntries = new List<ShadingEntry>();

    [SerializeField] private TextMeshPro[] targetTextRenderers = default;
    [SerializeField] private SpriteRenderer[] targetSpriteRenderers = default;

    public float Alpha
    {
        set => Apply((colorSetter, startColor) => colorSetter(startColor.SetAlpha(value)));
    }
    
    private void Awake()
    {
        foreach (var spriteRenderer in targetSpriteRenderers)
            shadingEntries.Add(ProduceEntry(spriteRenderer));

        foreach (var textRenderer in targetTextRenderers)
            shadingEntries.Add(ProduceEntry(textRenderer));
    }

    public IObservable<Unit> Fade(float toAlphaValue, TimeSpan withDuration)
    {
        return Observable.Create<Unit>(observer =>
        {
            TryNotifyingErrors(observer);

            var alpha = shadingEntries[0].colorGetter().a;
            var tween = DOTween.To(
                    () => alpha,
                    a =>
                    {
                        Alpha = a;
                        alpha = a;
                    },
                    toAlphaValue,
                    (float) withDuration.TotalSeconds)
                .OnComplete(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

            return Disposable.Create(() => tween.Kill());
        });
    }

    public void Tint(Color withColor, float byFactor)
    {
        Apply((colorSetter, startColor) => colorSetter(startColor.Tint(withColor, byFactor)));
    }

    public void Fog(Color withColor, float byFactor)
    {
        Apply((propertyBlock, startColor) =>
        {
            propertyBlock.SetColor(FogColorPropertyId, withColor);
            propertyBlock.SetFloat(FogIntensityPropertyId, byFactor);
        });
    }

    private static ShadingEntry ProduceEntry(SpriteRenderer forSpriteRenderer)
    {
        return new ShadingEntry(
            forSpriteRenderer.GetComponent<Renderer>(),
            () => forSpriteRenderer.color,
            color => forSpriteRenderer.color = color);
    }

    private static ShadingEntry ProduceEntry(Graphic forTextRenderer)
    {
        return new ShadingEntry(
            forTextRenderer.GetComponent<Renderer>(),
            () => forTextRenderer.color,
            color => forTextRenderer.color = color);
    }

    private void Apply(Action<Action<Color>, Color> colorTransform)
    {
        foreach (var entry in shadingEntries)
            entry.Apply(colorTransform);
    }

    private void Apply(Action<MaterialPropertyBlock, Color> colorTransform)
    {
        foreach (var entry in shadingEntries)
            entry.Apply(colorTransform);
    }

    private void TryNotifyingErrors(IObserver<Unit> observer)
    {
        if (shadingEntries.Count <= 0)
            observer.OnError(new Exception("[CardShader] No Entries provided for shading."));
    }
    
    private struct ShadingEntry
    {
        public readonly Func<Color> colorGetter;

        private readonly Renderer renderer;
        private readonly Action<Color> colorSetter;
        private readonly MaterialPropertyBlock materialPropertyBlock;

        public ShadingEntry(
            Renderer renderer,
            Func<Color> colorGetter,
            Action<Color> colorSetter
        )
        {
            this.renderer = renderer;
            this.colorGetter = colorGetter;
            this.colorSetter = colorSetter;

            materialPropertyBlock = GetPropertyBlock(renderer);
        }

        public void Apply(Action<Action<Color>, Color> colorTransform)
        {
            colorTransform(colorSetter, colorGetter());
        }

        public void Apply(Action<MaterialPropertyBlock, Color> colorTransform)
        {
            colorTransform(materialPropertyBlock, colorGetter());

            renderer.SetPropertyBlock(materialPropertyBlock);
        }

        private static MaterialPropertyBlock GetPropertyBlock(Renderer forRenderer)
        {
            var propBlock = new MaterialPropertyBlock();

            forRenderer.GetPropertyBlock(propBlock);

            return propBlock;
        }
    }
}