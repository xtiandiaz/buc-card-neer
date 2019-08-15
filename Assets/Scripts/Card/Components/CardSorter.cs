using UnityEngine;
using UnityEngine.Rendering;

public class CardSorter : MonoBehaviour
{
    [Space]
    [SerializeField] protected CardCover frontCover = default;
    [SerializeField] protected CardCover backCover = default;
    
    [Space]
    [SerializeField] private SortingGroup sortingGroup = default;
    
    public int Order
    {
        get => sortingGroup.sortingOrder;
        set
        {
            sortingGroup.sortingOrder = value;

            var shouldToggleFaceContent = value >= -2;

            frontCover.ToggleContent(shouldToggleFaceContent);
            backCover.ToggleContent(shouldToggleFaceContent);
        }
    }
}