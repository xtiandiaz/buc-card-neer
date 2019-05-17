using Zenject;

public class InspectorCardController : CardController
{
    public class Factory : PlaceholderFactory<IInspectorCard, IInspectorCardView, InspectorCardController>
    {
    }
    
    private readonly IInspectorCard model;
    private readonly IInspectorCardView view;
    
    public InspectorCardController(IInspectorCard model, IInspectorCardView view) : base(model, view)
    {
        this.model = model;
        this.view = view;
    }
}