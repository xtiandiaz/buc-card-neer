using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LoadingMenu : Menu
{
    [SerializeField] private Image indicator = default;

    private void Awake()
    {
        Observable.EveryUpdate()
            .Subscribe(_ => indicator.rectTransform.Rotate(0, 0, -45f * Time.deltaTime))
            .AddTo(this);
    }
}