using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CardValue : MonoBehaviour
{
    [SerializeField] private List<TextMeshPro> textRenderers = default;

    private bool hasValue;

    private Sequence pulsation;

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

        if (!hasValue)
        {
            hasValue = true;
            return;
        }

        pulsation?.Kill();
        pulsation = DOTween.Sequence();

        foreach (var renderer in textRenderers)
        {
            pulsation.Join(renderer.transform.DOPunchScale(Vector3.one * 1.05f, 0.5f, 3));
        }
    }
}