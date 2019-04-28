using UnityEngine;

public class CardSlotImage : MonoBehaviour
{
    [SerializeField] private uint capacity;
    [SerializeField] private SlotType type;

    public uint Capacity => capacity;
    public SlotType Type => type;
}
