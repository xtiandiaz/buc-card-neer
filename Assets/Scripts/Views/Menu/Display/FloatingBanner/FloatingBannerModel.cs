using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Model/UI/Floating Banner")]
public class FloatingBannerModel : ScriptableObject
{
    [SerializeField] private FloatingBannerType type = default;
    [SerializeField] private Sprite background = default;
    [SerializeField] private Image.Type backgroundType  = default;
    [SerializeField] private Color color = default;
    [SerializeField] private Vector2 size = default;

    public FloatingBannerType Type => type;
    public Sprite Background => background;
    public Image.Type BackgroundType => backgroundType;
    public Color Color => color;
    public Vector2 Size => size;
}