using System;

public interface IPlayerStats
{
    int HealthPoints { get; set; }
    int Coins { get; set; }
    
    IObservable<int> Health { get; }
    IObservable<int> Funds { get; }
}