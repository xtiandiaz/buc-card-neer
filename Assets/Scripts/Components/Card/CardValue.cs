using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICardValue
{
    void SetValue(int to);
    void SetValue(float to);
    void SetColor(Color to);
    void SetSorting(string withLayerName, int andOrder);
}

public class CardValue : MonoBehaviour, ICardValue
{
    [SerializeField] private List<TextMesh> textRenderers;
    [SerializeField] private string sortingLayerName;
    [SerializeField] private int sortingOrder;

    private List<MeshRenderer> MeshRenderers => textRenderers.Select(r => r.GetComponent<MeshRenderer>()).ToList();

    private void Awake()
    {
        SetSorting(sortingLayerName, sortingOrder);
    }

    public void SetValue(int to)
    {
        Set($"{to}");
    }
    
    public void SetValue(float to)
    {
        Set($"{to}");
    }

    public void SetColor(Color to)
    {
        textRenderers.ForEach(r => r.color = to);
    }

    public void SetSorting(string withLayerName, int andOrder)
    {
        MeshRenderers.ForEach(r =>
        {
            r.sortingLayerName = withLayerName;
            r.sortingOrder = andOrder;
        });
    }

    private void Set(string value)
    {
        textRenderers.ForEach(r => r.text = value);
    }
}