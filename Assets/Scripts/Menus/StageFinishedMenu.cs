using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public interface IStageFinishedMenu : IMenu
{
    IObservable<Unit> Feed(int withScore);
}

public class StageFinishedMenu : WorldSpaceMenu, IStageFinishedMenu
{
    [SerializeField] private ButtonText replayButton = default;
    [SerializeField] private ButtonText quitButton = default;

    [SerializeField] private Text scoreField = default;
    [SerializeField] private Text highScoreField = default;
    [SerializeField] private Text balanceField = default;

    private IAppNavigator appNavigator;
    private IPlayerStats playerStats;
    
    private int currentHighScore;
    private int currentBalance;

    [Inject]
    private void Initialize(
        IGameStatus gameStatus,
        IAppNavigator appNavigator,
        IPlayerStats playerStats
        )
    {
        this.appNavigator = appNavigator;
        this.playerStats = playerStats;

        currentHighScore = playerStats.HighScore;
        currentBalance = playerStats.Balance;

        scoreField.text = "0";
        highScoreField.text = $"{currentHighScore}";
        balanceField.text = $"{currentBalance}";
    }

    protected override void Start()
    {
        base.Start();

        replayButton.WhenClicked
            .Take(1)
            .Subscribe(_ => appNavigator.GoToGame())
            .AddTo(this);

        quitButton.WhenClicked
            .Take(1)
            .Subscribe(_ => appNavigator.GoToMainMenu())
            .AddTo(this);
    }

    public IObservable<Unit> Feed(int withScore)
    {
        return Observable.Create<Unit>(observer =>
            {
                Tween Punch(Transform transform)
                {
                    return transform.DOPunchScale(Vector3.one * 0.75f, 0.5f, 3);
                }

                Tween Count(int fromValue, int toValue, Text inField)
                {
                    var count = fromValue;

                    return DOTween.To(
                            () => count,
                            c =>
                            {
                                inField.text = $"{c}";
                                count = c;
                            },
                            toValue,
                            0.5f)
                        .SetEase(Ease.Linear);
                }
                
                var sequence = DOTween.Sequence();

                sequence.Append(Count(0, withScore, scoreField));
                sequence.Append(Punch(scoreField.transform));

                if (withScore > currentHighScore)
                {
                    playerStats.HighScore = withScore;
                    
                    sequence.Append(Count(currentHighScore,withScore, highScoreField));
                    sequence.Append(Punch(highScoreField.transform));
                }

                var newBalance = currentBalance + withScore;
                playerStats.Balance = newBalance;
                
                sequence.Append(Count(currentBalance,newBalance, balanceField));
                sequence.Append(Punch(balanceField.transform));

                sequence.OnComplete(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

                return Disposable.Create(() => sequence.Kill());
            })
            .DelaySubscription(TimeSpan.FromSeconds(0.5));
    }
}
