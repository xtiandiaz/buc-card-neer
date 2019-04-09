using Zenject;

public enum AbilityType
{
    StealthBoots,
    HawkEye,
    SuckerPunch,
    Decoy,
    MithrilVest
}

public interface IAbilityCard
{
    AbilityType AbilityType { get; }
    int Index { get; }
}

public class AbilityCard : Card, IAbilityCard
{
    public class Factory : PlaceholderFactory<AbilityType, int, AbilityCard>
    {
    }

    private AbilityCard(AbilityType abilityType, int sequenceNumber) : base(CardType.Ability)
    {
        AbilityType = abilityType;
        Index = sequenceNumber;
    }

    public AbilityType AbilityType { get; }
    public int Index { get; }
}