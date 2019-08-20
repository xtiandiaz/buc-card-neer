using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardShader : MonoBehaviour
{
    private static readonly int FogColorPropertyId = Shader.PropertyToID("_FogColor");
    private static readonly int FogIntensityPropertyId = Shader.PropertyToID("_FogIntensity");

    private readonly List<ShadingEntry> shadingEntries = new List<ShadingEntry>();

    [SerializeField] private TextMeshPro[] targetTextRenderers = default;
    [SerializeField] private SpriteRenderer[] targetSpriteRenderers = default;

    private Color fogColor = Color.black;
    private float fogIntensity;

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

    public Tween Fade(float toAlphaValue, float withDuration)
    {
        if (shadingEntries.Count <= 0) 
            throw new Exception("[CardShader] No Entries provided for shading.");

        var alpha = shadingEntries[0].colorGetter().a;
        return DOTween.To(
            () => alpha,
            a =>
            {
                Alpha = a;
                alpha = a;
            },
            toAlphaValue,
            withDuration);
    }

    public void Tint(Color withColor, float byFactor)
    {
        Apply((colorSetter, startColor) => colorSetter(startColor.Tint(withColor, byFactor)));
    }

    public Sequence Fog(Color withColor, float byFactor, Ease withEase, float andDuration)
    {
        var sequence = DOTween.Sequence();

        sequence.Append(DOTween.To(
            () => fogColor,
            color =>
            {
                Apply((propertyBlock, startColor) => propertyBlock.SetColor(FogColorPropertyId, color));
                fogColor = color;
            },
            withColor,
            andDuration));

        sequence.Join(DOTween.To(
            () => fogIntensity,
            intensity =>
            {
                Apply((propertyBlock, startColor) => propertyBlock.SetFloat(FogIntensityPropertyId, intensity));
                fogIntensity = intensity;
            },
            byFactor,
            andDuration));

        sequence.SetEase(withEase);

        return sequence;
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