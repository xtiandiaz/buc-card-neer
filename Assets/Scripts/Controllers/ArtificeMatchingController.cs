using System;
using UniRx;

public interface IArtificeMatchingController : IDisposable
{
    IObservable<ArtificeType> WhenMatched { get; }
    IObservable<ArtificeType> WhenActed { get; }
    
    bool CanMatch(IArtificeCard source, ICard withDestination);
    
    IObservable<Unit> Match(IArtificeCard source, ICard withDestination);
}

public class ArtificeMatchingController : IArtificeMatchingController
{
    private readonly Subject<ArtificeType> acting = new Subject<ArtificeType>();
    private readonly Subject<ArtificeType> matching = new Subject<ArtificeType>();

    private readonly IPlayerCard player;
    private readonly IBoard board;

    private ArtificeMatchingController(
        IPlayerCard player,
        IBoard board
        )
    {
        this.player = player;
        this.board = board;
    }

    public IObservable<ArtificeType> WhenMatched => matching;
    public IObservable<ArtificeType> WhenActed => acting;

    public bool CanMatch(IArtificeCard source, ICard withDestination)
    {
        switch (source.ArtificeType)
        {
            case ArtificeType.MidasTouch:
                return withDestination != player;
            case ArtificeType.TraderSpell:
                return withDestination.IsMerchant;
            default:
                return false;
        }
    }

    public IObservable<Unit> Match(IArtificeCard artifice, ICard withDestination)
    {
        return Observable.Create<Unit>(observer =>
        {
            switch (artifice.ArtificeType)
            {
                case ArtificeType.MidasTouch:

                    player.Credit(withDestination.Value + player.Value);

                    return artifice.Destroy()
                        .Merge(withDestination.Destroy())
                        .Subscribe(observer);

                case ArtificeType.TraderSpell:

                    var desiredSuit = board.Ship.Storage.Peek()?.Suit;

                    return artifice.Destroy()
                        .DoOnSubscribe(() => ((IMerchantCard)withDestination).Resuit(desiredSuit))
                        .Subscribe(observer);

                default:

                    observer.OnError(
                        new Exception($"Couldn't match with Artifice '{artifice.ArtificeType}'"));

                    return Disposable.Empty;
            }
        })
        .DoOnSubscribe(() => matching.OnNext(artifice.ArtificeType))
        .DoOnCompleted(() => acting.OnNext(artifice.ArtificeType));
    }

    public void Dispose()
    {
        acting.Dispose();
        matching.Dispose();
    }
}