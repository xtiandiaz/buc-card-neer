using Zenject;

public class SlotBoarding : Slot
{
    public class Factory : PlaceholderFactory<uint, SlotBoarding>
    {
    }

    public SlotBoarding(uint capacity) : base(SlotType.Boarding, capacity)
    {
    }

    public override CardType EntryMask => CardType.Foe | CardType.Merchant | CardType.Treasure;
}