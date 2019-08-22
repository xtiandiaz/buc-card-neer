using UnityEngine;
using UnityEngine.UI;

public class LocalizedField : MonoBehaviour
{
    [SerializeField] protected string localizedTextKey = default;

    protected virtual void Start()
    {
        Localizator.Hook(GetComponent<Text>(), localizedTextKey);
    }
}