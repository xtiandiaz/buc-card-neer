using System;

public interface IPlayerStats : IPlayerData
{
    int HighScore { get; set; }
    int Balance { get; set; }
}

public class PlayerStats : PlayerData, IPlayerStats
{
    private const string HighScorePrefKey = "HighScore";
    private const string BalancePrefKey = "Balance";

    public int HighScore
    {
        get => GetInt(HighScorePrefKey, 0);
        set
        {
            SetInt(HighScorePrefKey, Math.Max(value, HighScore));
            Save();
        }
    }

    public int Balance
    {
        get => GetInt(BalancePrefKey, 0);
        set
        {
            SetInt(BalancePrefKey, value);
            Save();
        }
    }

    public override void Clear()
    {
        Delete(HighScorePrefKey);
        Delete(BalancePrefKey);
    }
}