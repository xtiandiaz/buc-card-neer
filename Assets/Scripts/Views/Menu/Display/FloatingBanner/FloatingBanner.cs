using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FloatingBanner : MonoBehaviour
{
    private const float FadeDuration = 0.25f;
    
    [SerializeField] private Image background = default;
    [SerializeField] private TextMeshProUGUI label = default;
    [SerializeField] private CanvasGroup canvasGroup = default;
    
    private Sequence display;
    
    public enum DisplayMode
    {
        FadeIn,
        FadeInUpward,
        FadeInDownward
    }
    
    public enum HidingMode
    {
        FadeOut,
        FadeOutDownward,
        FadeOutUpward
    }
    
    public RectTransform RectTransform => (RectTransform) transform;
    
    public void Initialize(FloatingBannerModel withModel, string andText)
    {
        background.sprite = withModel.Background;
        background.type = withModel.BackgroundType;
        background.color = withModel.Color;
        
        var rectTransform = background.rectTransform;
        rectTransform.sizeDelta = withModel.Size;

        label.text = andText;

        canvasGroup.alpha = 0;
    }

    public void Show(DisplayMode withMode, float duration, bool andDestroyAutomatically)
    {
        Show(withMode, duration);

        if (!andDestroyAutomatically)
            return;
        
        display.Join(canvasGroup.DOFade(0, FadeDuration)
                .SetDelay(duration - FadeDuration))
            .OnComplete(() => Destroy(gameObject));
    }
    
    public void Show(DisplayMode withMode, float andDuration = 1f)
    {
        display?.Kill();
        display = DOTween.Sequence();
        
        transform.localScale = Vector3.zero;

        display.Append(canvasGroup.DOFade(1f, FadeDuration));
        
        display.Join(transform.DOScale(1f, FadeDuration)
            .SetEase(Ease.OutBack));

        switch (withMode)
        {
            case DisplayMode.FadeInUpward:
                
                display.Append(RectTransform.DOAnchorPos3DY(RectTransform.anchoredPosition3D.y + andDuration * 10f, andDuration)
                    .SetEase(Ease.Linear));
                
                break;
            case DisplayMode.FadeInDownward:
                
                display.Append(RectTransform.DOAnchorPos3DY(RectTransform.anchoredPosition3D.y - andDuration * 10f, andDuration)
                    .SetEase(Ease.Linear));
                
                break;
        }
    }

    public void Hide(HidingMode withMode, float withDelay = 0f)
    {
        display?.Kill();
        display = DOTween.Sequence();

        const float duration = 1f;

        switch (withMode)
        {
            case HidingMode.FadeOutDownward:

                display.Join(RectTransform.DOAnchorPos3DY(RectTransform.anchoredPosition3D.y - 10f, duration)
                    .SetEase(Ease.Linear));
                
                break;
            case HidingMode.FadeOutUpward:
                
                display.Join(RectTransform.DOAnchorPos3DY(RectTransform.anchoredPosition3D.y + 10f, duration)
                    .SetEase(Ease.Linear));
                
                break;
        }

        display.Join(canvasGroup.DOFade(0, duration))
            .OnComplete(() => Destroy(gameObject));

        display.SetDelay(withDelay);
    }
    
    public class Factory : PlaceholderFactory<FloatingBanner>
    {
    }
}