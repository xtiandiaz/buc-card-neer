public interface IResourceBuyer : IResourceAgent
{
    int Coins { get; }
    
    bool CanBuy(IResourceCard resourceCard);
    void Buy(IResourceCard resourceCard);
}