using Zenject;

public interface IStage
{
    int SupplySize { get; }
    int BaggageSize { get; }
}

public class Stage : IStage
{
    private Stage(
        IStageModel model
    )
    {
        SupplySize = (int) model.SupplySize;
        BaggageSize = (int) model.BaggageSize;
    }

    public int SupplySize { get; }
    public int BaggageSize { get; }

    public class Factory : PlaceholderFactory<IStageModel, Stage>
    {        
    }
}
