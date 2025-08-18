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

    // ����¼�¼
    public void AddResult(int kills, float time)
    {
        GameResult newResult = new GameResult(kills, time);
        leaderboard.Add(newResult);

        // ���������򣨴Ӹߵ��ͣ�
        leaderboard = leaderboard
            .OrderByDescending(r => r.CalculateScore())
            .ToList();

        // ֻ����ǰ10��
        if (leaderboard.Count > 10)
        {
            leaderboard = leaderboard.Take(10).ToList();
        }

        SaveLeaderboard();
    }

    // ��ȡ���а�
    public List<GameResult> GetLeaderboard()
    {
        return leaderboard;
    }

    // ���浽�ļ�
    public void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(new LeaderboardData(leaderboard));
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        File.WriteAllText(path, json);
    }

    // ���ļ�����
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

    // �������������л��б�
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