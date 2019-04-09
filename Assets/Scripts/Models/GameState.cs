using System.Collections.Generic;

public class GameState
{
    private GameState()
    {
        foreach (var abilityType in AbilityIndex)
        {
            AbilityInventory[abilityType] = false;
        }
    }
    
    public AbilityType[] AbilityIndex { get; } =
    {
        AbilityType.StealthBoots, 
        AbilityType.HawkEye, 
        AbilityType.SuckerPunch, 
        AbilityType.MithrilVest,
        AbilityType.Decoy
    };
    
    private Dictionary<AbilityType, bool> AbilityInventory { get; } = new Dictionary<AbilityType, bool>();
}