using System;
using UniRx;
using Zenject;

public interface IGameStatus : IInitializable, IDisposable
{
    bool PlayerDidStashItem { get; set; }
    bool PlayerDidStashTool { get; set; }
    bool DidSupplyOnce { get; set; }
    
    IObservable<int> UndealtCardCount { get; }
    IObservable<Unit> WhenPlayerLost { get; }
    IObservable<int> WhenPlayerWon { get; }
    
    IObservable<Unit> WhenPlayerShot { get; set; }
    IObservable<Unit> WhenPlayerUnlockedAndHandledCard { get; set; }
    IObservable<Unit> WhenPlayerAttackedOnBoard { get; set; }
    IObservable<Unit> WhenPlayerConfronted { get; set; }
}