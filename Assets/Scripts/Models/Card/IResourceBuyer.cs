public interface IResourceBuyer : IResourceAgent
{
    int Coins { get; }
    
    bool CanBuy(IResourceCard resource);
    void Buy(IResourceCard resourceCard);
}