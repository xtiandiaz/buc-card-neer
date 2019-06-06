using System.Collections.Generic;
using UnityEngine;

public class CardLabel : MonoBehaviour
{
    [SerializeField] private List<TextMesh> textRenderers;

    public Color Color
    {
        set => textRenderers.ForEach(r => r.color = value);
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

    public void ToggleVisibility(bool toValue)
    {
        gameObject.SetActive(toValue);
    }
}