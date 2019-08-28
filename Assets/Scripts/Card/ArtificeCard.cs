using Zenject;

public interface IArtificeCard : ICard
{
    ArtificeType ArtificeType { get; }
    
    bool IsLodgeable { get; }
}

public class ArtificeCard : Card, IArtificeCard
{
    private ArtificeCard(IArtificeCardModel model, IArtificeCardView view) 
        : base(model, view)
    {
        ArtificeType = model.ArtificeType;
        IsBoarded = true;
        
        view.FaceColor = model.FaceColor;
        view.Name = model.Name;
    }
    
    public ArtificeType ArtificeType { get; }

    public bool IsLodgeable => (ArtificeType & (ArtificeType.Catapult)) != 0;

    public new class Factory : PlaceholderFactory<IArtificeCardModel, IArtificeCardView, ArtificeCard>
    {
    }
}