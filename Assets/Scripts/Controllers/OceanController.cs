using Zenject;

public class OceanController
{
    public class Factory : PlaceholderFactory<IOcean, IOceanView, OceanController>
    {
    }

    private OceanController(IOcean model, IOceanView view)
    {
    }
}