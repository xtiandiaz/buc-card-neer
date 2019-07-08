using System.Collections.Generic;
using UnityEngine;

public interface ISortingSet
{
    int SortingOrder { set; }
}

public class SortingSet : MonoBehaviour, ISortingSet
{
    [SerializeField] private SpriteRenderer[] targetSpriteRenderers = default;

    [Header("Text")] 
    [SerializeField] private MeshRenderer[] targetTextRenderers = default;
    [SerializeField] private int textDefaultSortingOrder = default;
    [SerializeField] private string textSortingLayerName = default;

    private Dictionary<Object, int> defaultSortingIndex;

    public int SortingOrder
    {
        set
        {
            foreach (var spriteRenderer in targetSpriteRenderers)
                spriteRenderer.sortingOrder = value + DefaultSortingIndex[spriteRenderer];
            
            foreach (var textRenderer in targetTextRenderers)
                textRenderer.sortingOrder = value + DefaultSortingIndex[textRenderer];
        }
    }

    private Dictionary<Object, int> DefaultSortingIndex
    {
        get
        {
            if (defaultSortingIndex != null)
                return defaultSortingIndex;
            
            defaultSortingIndex = new Dictionary<Object, int>();
            
            foreach (var spriteRenderer in targetSpriteRenderers)
                defaultSortingIndex.Add(spriteRenderer, spriteRenderer.sortingOrder);

            foreach (var textRenderer in targetTextRenderers)
            {
                textRenderer.sortingOrder = textDefaultSortingOrder;
                textRenderer.sortingLayerName = textSortingLayerName;
                
                defaultSortingIndex.Add(textRenderer, textDefaultSortingOrder);
            } 

            return defaultSortingIndex;
        }
    }
}