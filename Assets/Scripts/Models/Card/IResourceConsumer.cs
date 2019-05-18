public interface IResourceConsumer : IResourceAgent
{
    bool CanConsume(IResourceCard resourceCard);
    void Consume(IResourceCard resourceCard);
}