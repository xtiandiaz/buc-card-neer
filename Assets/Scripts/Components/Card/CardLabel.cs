using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardLabel : MonoBehaviour
{
    [SerializeField] private List<TextMesh> textRenderers;
    [SerializeField] private string sortingLayerName;
    [SerializeField] private int sortingOrder;

    private List<MeshRenderer> MeshRenderers => textRenderers.Select(r => r.GetComponent<MeshRenderer>()).ToList();

    public Color Color
    {
        set => textRenderers.ForEach(r => r.color = value);
    }
    
    private void Awake()
    {
        SetSorting(sortingLayerName, sortingOrder);
    }

    public void SetValue(int to)
    {
        SetValue($"{to}");
    }
    
    public void SetValue(float to)
    {
        SetValue($"{to}");
    }

    public void SetValue(string to)
    {
        textRenderers.ForEach(r => r.text = to);
    }

    private void SetSorting(string withLayerName, int andOrder)
    {
        MeshRenderers.ForEach(r =>
        {
            r.sortingLayerName = withLayerName;
            r.sortingOrder = andOrder;
        });
    }
}