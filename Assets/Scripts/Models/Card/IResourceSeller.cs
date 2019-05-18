public interface IResourceSeller : IResourceAgent
{
    int Coins { get; }
    
    bool CanSell(IResourceCard resourceCard);
    void Sell(IResourceCard resourceCard, IMerchantCard toMerchant);
}