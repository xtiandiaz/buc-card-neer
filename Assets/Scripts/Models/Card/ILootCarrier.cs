public interface ILootCarrier
{
    bool IsDead { get; }
    
    int GetLoot();
}