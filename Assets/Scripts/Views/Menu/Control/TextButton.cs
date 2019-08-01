using UnityEngine;
using UnityEngine.UI;

public class TextButton : Selectable
{
    [SerializeField] private Text label = default;

    public string Label
    {
        set => label.text = value;
    }
}