using UnityEngine;

public class CardSlotImage : MonoBehaviour
{
    [SerializeField] private uint capacity;
    [SerializeField] private CardSlotType type;

    public uint Capacity => capacity;
    public CardSlotType Type => type;
}
