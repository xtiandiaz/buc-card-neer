using System;
using UnityEngine;

public class GamePalette : MonoBehaviour
{
    [SerializeField] private Color health;
    [SerializeField] private Color stamina;
    [SerializeField] private Color defense;
    [SerializeField] private Color ability1;
    [SerializeField] private Color ability2;
    [SerializeField] private Color ability3;
    [SerializeField] private Color ability4;
    [SerializeField] private Color ability5;
    [SerializeField] private Color baddie;
    
    public Color Health => health;
    public Color Stamina => stamina;
    public Color Defense => defense;
    public Color Ability1 => ability1;
    public Color Ability2 => ability2;
    public Color Ability3 => ability3;
    public Color Ability4 => ability4;
    public Color Ability5 => ability5;
    public Color Baddie => baddie;
}