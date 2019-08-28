using System.Collections.Generic;
using UnityEngine;

public interface IArtificeCatalog
{
    ArtificeCardModel this[ArtificeType key] { get; }
    
    void Index();
}

[CreateAssetMenu(menuName = "Model/Device Catalog")]
public class ArtificeCatalog : ScriptableObject, IArtificeCatalog
{
    [SerializeField] private ArtificeCardModel[] models = default;
    
    private Dictionary<ArtificeType, ArtificeCardModel> index;
    
    public ArtificeCardModel this[ArtificeType key] => Instantiate(index[key]);

    public void Index()
    {
        if (index != null)
            return;
        
        index = new Dictionary<ArtificeType, ArtificeCardModel>();
        
        foreach (var model in models)
            index.Add(model.ArtificeType, model);
    }
}