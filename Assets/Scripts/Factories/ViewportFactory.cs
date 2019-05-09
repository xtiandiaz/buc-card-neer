using Zenject;

public interface IViewportFactory : IFactory<Viewport>
{
}

public class ViewportFactory : IViewportFactory
{
    private readonly IViewportProvider provider;

    private ViewportFactory(IViewportProvider provider)
    {
        this.provider = provider;
    }
    
    public Viewport Create()
    {
        return provider.GetViewport(0);
    }
}