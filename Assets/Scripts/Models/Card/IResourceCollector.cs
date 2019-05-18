public interface IResourceCollector : IResourceAgent
{
    bool CanCollect(IResourceCard resourceCard);
    void Collect(IResourceCard resourceCard);
}