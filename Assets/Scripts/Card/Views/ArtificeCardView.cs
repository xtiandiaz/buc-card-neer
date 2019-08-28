using UnityEngine;

public interface IArtificeCardView : ICardView
{
    Color FaceColor { set; }
}

public class ArtificeCardView : CardView, IArtificeCardView
{
    public Color FaceColor
    {
        set => customizer.FrontCoverColor = value;
    }
}