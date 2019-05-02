using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICardShader
{
    void Fade(float toAlphaValue);
    void Tint(Color withColor, float byFactor);
    void Fog(Color withColor, float byFactor);
}

public class CardShader : MonoBehaviour, ICardShader
{
    private static readonly int RendererColor = Shader.PropertyToID("_RendererColor");
    private static readonly int FogColor = Shader.PropertyToID("_FogColor");
    private static readonly int FogIntensity = Shader.PropertyToID("_FogIntensity");
    
    private readonly Dictionary<Renderer, (MaterialPropertyBlock, Color)> shadingIndex 
        = new Dictionary<Renderer, (MaterialPropertyBlock, Color)>();

    [SerializeField] private Renderer[] targetRenderers;

    private void Awake()
    {
        foreach (var renderer in targetRenderers)
        {
            var propBlock = new MaterialPropertyBlock();
            
            renderer.GetPropertyBlock(propBlock);
            
            shadingIndex.Add(renderer, (propBlock, propBlock.GetColor(RendererColor)));
        }
    }
    
    public void Fade(float toAlphaValue)
    {
        Apply((propertyBlock, originalColor) => 
            propertyBlock.SetColor(RendererColor, originalColor.SetAlpha(toAlphaValue)));
    }
    
    public void Tint(Color withColor, float byFactor)
    {
        Apply((propertyBlock, originalColor) => 
            propertyBlock.SetColor(RendererColor, originalColor.Tint(withColor, byFactor)));
    }
    
    public void Fog(Color withColor, float byFactor)
    {
        Apply((propertyBlock, originalColor) =>
            {
                propertyBlock.SetColor(FogColor, withColor);
                propertyBlock.SetFloat(FogIntensity, byFactor);
            });
    }

    private void Apply(Action<MaterialPropertyBlock, Color> colorTransform)
    {
        foreach (var entry in shadingIndex)
        {
            if (!entry.Key.gameObject.activeInHierarchy)
                continue;

            var (propertyBlock, originalColor) = entry.Value;

            colorTransform(propertyBlock, originalColor);
            
            entry.Key.SetPropertyBlock(propertyBlock);
        }
    }
}