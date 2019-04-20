using System;
using UnityEngine;

public class GamePalette : MonoBehaviour
{
    [SerializeField] private Color slotHighlight;
    [SerializeField] private Color pirate;
    
    public Color SlotHighlight => slotHighlight;
    public Color Pirate => pirate;
}