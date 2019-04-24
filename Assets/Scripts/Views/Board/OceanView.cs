using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class OceanView : MonoBehaviour
{
    [SerializeField] private List<CardSlotView> slots;
    [SerializeField] private Transform slotWrapper;
    [SerializeField] private MeshRenderer background;
    
    private GameSettings settings;
    private Transform backgroundTransform;

    public float Height => settings.CardSize.y;

    [Inject]
    private void Construct(
        GameSettings settings
    )
    {
        this.settings = settings;
        backgroundTransform = background.transform;
    }

    public void Initialize(float boardHeight)
    {
        backgroundTransform.localScale = new Vector3(boardHeight * 2f, boardHeight * 2f, 1f);
    }

    public void ToggleProjection(bool on)
    {
        background.transform.localRotation = Quaternion.Euler(on ? 60f : 0, 0, 0);
        
        backgroundTransform.localPosition = new Vector3(
                                                0, 
                                                on ? settings.CardSize.y : 0, 
                                                settings.CardSize.y);
        
        slotWrapper.gameObject.SetActive(on);
    }
}