using System.Collections.Generic;
using UnityEngine;

public interface IDeviceCatalog
{
    DeviceCardModel this[DeviceType key] { get; }
    
    void Index();
}

[CreateAssetMenu(menuName = "Model/Device Catalog")]
public class DeviceCatalog : ScriptableObject, IDeviceCatalog
{
    [SerializeField] private DeviceCardModel[] models = default;
    
    private Dictionary<DeviceType, DeviceCardModel> index;
    
    public DeviceCardModel this[DeviceType key] => Instantiate(index[key]);

    public void Index()
    {
        if (index != null)
            return;
        
        index = new Dictionary<DeviceType, DeviceCardModel>();
        
        foreach (var model in models)
            index.Add(model.DeviceType, model);
    }
}