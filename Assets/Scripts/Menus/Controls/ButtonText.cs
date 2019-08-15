using UnityEngine;
using UnityEngine.UI;

public class ButtonText : CustomButton
{
    [SerializeField] private Text label = default;

    public string Label
    {
        set => label.text = value;
    }
}