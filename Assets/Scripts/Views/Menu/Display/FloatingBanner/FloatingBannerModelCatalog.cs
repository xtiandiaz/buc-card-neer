using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Model/UI/Floating Banner Catalog")]
public class FloatingBannerModelCatalog : ScriptableObject
{
    [SerializeField] private FloatingBannerModel[] models = default;

    private Dictionary<FloatingBannerType, FloatingBannerModel> index;

    public FloatingBannerModel this[FloatingBannerType type] => index[type];
    
    public void Index()
    {
        if (index != null)
            return;
        
        index = new Dictionary<FloatingBannerType, FloatingBannerModel>();
        
        foreach (var model in models)
            index[model.Type] = model;
    } 
}