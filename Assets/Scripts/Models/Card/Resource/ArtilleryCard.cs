using UnityEngine;

public interface IArtilleryCard
{
    bool IsArmed { get; }
    
    void Arm();
}

[CreateAssetMenu(fileName = "CardArtillery", menuName = "Game/Card/Artillery", order = 1)]
public class ArtilleryCard : ResourceCard, IArtilleryCard
{
    public bool IsArmed { get; private set; }
    
    public void Arm()
    {
        IsArmed = true;
    }
}