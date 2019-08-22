using TMPro;
using UnityEngine;

public class LocalizedTMPField : MonoBehaviour
{
    [SerializeField] protected string localizedTextKey = default;

    protected virtual void Start()
    {
        Localizator.Hook(GetComponent<TMP_Text>(), localizedTextKey);
    }
}