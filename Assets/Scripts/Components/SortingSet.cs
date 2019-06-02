using System.Collections.Generic;
using UnityEngine;

public interface ISortingSet
{
    int SortingOrder { set; }
}

public class SortingSet : MonoBehaviour, ISortingSet
{
    [SerializeField] private SpriteRenderer[] targetSpriteRenderers;

    [Header("Text")] 
    [SerializeField] private MeshRenderer[] targetTextRenderers;
    [SerializeField] private int textDefaultSortingOrder;
    [SerializeField] private string textSortingLayerName;

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