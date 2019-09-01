using System;
using UniRx;

public interface IConfrontationController : IDisposable
{
    IObservable<CardType> WhenPlayerConfronted { get; }

    bool CanConfront(ICard foe);
    
    IObservable<Unit> Confront(ICard card);
}

public class ConfrontationController : IConfrontationController
{
    private readonly Subject<CardType> confronting = new Subject<CardType>();
    
    private readonly IPlayerCard player;

    private ConfrontationController(IPlayerCard player)
    {
        this.player = player;
    }

    public IObservable<CardType> WhenPlayerConfronted => confronting;

    public bool CanConfront(ICard foe)
    {
        return foe.IsPirate || foe.IsMonster;
    }

    public IObservable<Unit> Confront(ICard foe)
    {
        return Observable.Create<Unit>(observer =>
        {
            if ((foe.Type & (CardType.Pirate)) != 0)
            {
                return ConfrontAgent(foe)
                    .Subscribe(observer);
            }
            else if (foe.IsMonster)
            {
                return ConfrontMonster(foe)
                    .Subscribe(observer);
            }

            observer.OnError(new Exception($"Cannot confront with Card ({foe.Type})"));
            
            return Disposable.Empty;            
        });
    }

    public void Dispose()
    {
        confronting.Dispose();
    }

    private IObservable<Unit> ConfrontAgent(ICard agent)
    {
        return Observable.Create<Unit>(observer =>
        {
            var agentHealth = agent.Value;

            confronting.OnNext(agent.Type);

            return agent.Hit(Math.Min(player.Value, agentHealth))
                .Merge(player.Hit(agentHealth)
                    .Do(_ =>
                    {
                        if (player.HealthPoints <= 0)
                            return; 

                        switch(agent.Type)
                        {
                            case CardType.Pirate:
                                player.Credit(agent.OriginalValue);
                                break;
                        }                            
                    }))
                .LastOrDefault()
                .Subscribe(observer);
        });
    }

    private IObservable<Unit> ConfrontMonster(ICard monster)
    {
        return Observable.Create<Unit>(observer =>
        {
            if (!monster.IsMonster)
            {
                observer.OnError(new Exception($"Card to confront: '{monster.Name}' ({monster.Type}), is not a Monster."));
                return Disposable.Empty;
            }

            var monsterHealth = monster.LockValue;
            
            monster.Hack(Math.Min(player.Value, monsterHealth));
            confronting.OnNext(CardType.Monster);

            return player.Hit(monsterHealth)
                .Subscribe(observer);
        });
    }
}