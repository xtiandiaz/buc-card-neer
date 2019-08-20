using UnityEngine;
using UnityEngine.UI;

public class ButtonText : CustomButton
{
    [SerializeField] protected Text label = default;

    public void SetText(string text)
    {
        label.text = text;
    }
}