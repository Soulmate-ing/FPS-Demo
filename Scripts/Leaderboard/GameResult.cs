using System;

[Serializable]
public class GameResult
{
    public int kills;
    public float time;
    public DateTime timestamp;

    public GameResult(int kills, float time)
    {
        this.kills = kills;
        this.time = time;
        this.timestamp = DateTime.Now;
    }

    // 计算总分（只考虑击杀数和时间）
    public int CalculateScore()
    {
        // 击杀数权重: 100，时间权重: -1（时间越短越好）
        return kills * 100 + (int)(-time);
    }
}