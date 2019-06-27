using System.Collections.Generic;
using UnityEngine;

public class CardValue : MonoBehaviour
{
    [SerializeField] private List<TextMesh> textRenderers;

    public Color Color
    {
        set => textRenderers.ForEach(r => r.color = value);
    }

    public void SetValue(int to, string withPrefix = null, string andSuffix = null)
    {
        SetValue($"{withPrefix}{to}{andSuffix}");
    }
    
    public void SetValue(float to, string withPrefix = null, string andSuffix = null)
    {
        SetValue($"{withPrefix}{to}{andSuffix}");
    }

    public void ToggleVisibility(bool toValue)
    {
        textRenderers.ForEach(r => r.GetComponent<MeshRenderer>().enabled = toValue);
    }
    
    private void SetValue(string to)
    {
        textRenderers.ForEach(r => r.text = to);
    }
}