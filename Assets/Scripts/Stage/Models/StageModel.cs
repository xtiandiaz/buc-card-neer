using UnityEngine;

public interface IStageModel
{
    StageKey Key { get; }
    IDeckModel Deck { get; }
    uint SupplySize { get; }
    uint BaggageSize { get; }
}

[CreateAssetMenu(menuName = "Model/Stage/Stage")]
public class StageModel : ScriptableObject, IStageModel
{
    [SerializeField] private StageKey key = default;
    [SerializeField] private DeckModel deck = default;
    [SerializeField] private uint supplySize = default;
    [SerializeField] private uint baggageSize = default;

    public StageKey Key => key;
    public IDeckModel Deck => deck;
    public uint SupplySize => supplySize;
    public uint BaggageSize => baggageSize;
}