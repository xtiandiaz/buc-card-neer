using UnityEngine;

public interface IArtificeCardModel : ICardModel
{
    ArtificeType ArtificeType { get; }
    Color FaceColor { get; }
}

[CreateAssetMenu(menuName = "Model/Card/Device")]
public class ArtificeCardModel : CardModel, IArtificeCardModel
{
    [SerializeField] private ArtificeType artificeType = default;
    [SerializeField] private Color faceColor = default;

    public override CardType Type => CardType.Artifice;
    public ArtificeType ArtificeType => artificeType;
    public Color FaceColor => faceColor;
}