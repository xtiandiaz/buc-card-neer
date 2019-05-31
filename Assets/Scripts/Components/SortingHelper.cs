using UnityEngine;

public class SortingHelper : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] targetSpriteRenderers;

    public int SortingOrder
    {
        set
        {
            foreach (var spriteRenderer in targetSpriteRenderers)
                spriteRenderer.sortingOrder = value;
        }
    }
}