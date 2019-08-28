using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Model/Stage/Catalog")]
public class StageCatalog : ScriptableObject
{
    [SerializeField] private StageModel[] models = default;
    
    private Dictionary<StageKey, StageModel> index;
    
    public IStageModel this[StageKey key] => Instantiate(index[key]);

    public void Index()
    {
        if (index != null)
            return;
        
        index = new Dictionary<StageKey, StageModel>();
        
        foreach (var model in models)
            index.Add(model.Key, model);
    }
}