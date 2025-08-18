using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    private static LeaderboardManager instance;
    public static LeaderboardManager Instance => instance;

    private List<GameResult> leaderboard = new List<GameResult>();
    private const string SaveFileName = "leaderboard.json";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLeaderboard();
    }

    // 添加新记录
    public void AddResult(int kills, float time)
    {
        GameResult newResult = new GameResult(kills, time);
        leaderboard.Add(newResult);

        // 按分数排序（从高到低）
        leaderboard = leaderboard
            .OrderByDescending(r => r.CalculateScore())
            .ToList();

        // 只保留前10名
        if (leaderboard.Count > 10)
        {
            leaderboard = leaderboard.Take(10).ToList();
        }

        SaveLeaderboard();
    }

    // 获取排行榜
    public List<GameResult> GetLeaderboard()
    {
        return leaderboard;
    }

    // 保存到文件
    public void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(new LeaderboardData(leaderboard));
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        File.WriteAllText(path, json);
    }

    // 从文件加载
    private void LoadLeaderboard()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            leaderboard = data.results;
        }
    }

    // 辅助类用于序列化列表
    [Serializable]
    private class LeaderboardData
    {
        public List<GameResult> results;

        public LeaderboardData(List<GameResult> results)
        {
            this.results = results;
        }
    }
}