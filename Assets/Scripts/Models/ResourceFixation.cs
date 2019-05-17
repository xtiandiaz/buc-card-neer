using System;
using UnityEngine;

public interface IResourceFixation
{
    Suit Suit { get; }
    int Degree { get; }
}

[Serializable]
public class ResourceFixation : IResourceFixation
{
    [SerializeField] private Suit suit;
    [SerializeField] [Range(2, 4)] private int degree;

    public Suit Suit => suit;
    public int Degree  => degree;
}