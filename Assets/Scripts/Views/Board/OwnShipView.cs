using UnityEngine;
using Zenject;

public class OwnShipView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    
    private GameSettings settings;

    public float Height => background.size.y;
    
    [Inject]
    private void Construct(
        GameSettings settings
    )
    {
        this.settings = settings;
    }


    public void Initialize()
    {
    }
}