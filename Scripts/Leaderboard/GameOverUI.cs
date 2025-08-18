using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    public GameObject gameOverPanel;
    public Text resultText;
    public Text killsText;
    public Text timeText;

    // 直接引用场景中的10个条目
    public LeaderboardEntryUI[] leaderboardEntries = new LeaderboardEntryUI[10];
    private void Awake()
    {
        Instance = this;
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(string reason, int kills, float time)
    {
        gameOverPanel.SetActive(true);

        // 显示本次游戏结果
        resultText.text = reason;
        killsText.text = $"Kill: {kills}";
        timeText.text = $"Time: {FormatTime(time)}";

        // 更新排行榜
        PopulateLeaderboard();
    }

    public void PopulateLeaderboard()
    {
        // 获取排行榜数据
        List<GameResult> leaderboard = LeaderboardManager.Instance.GetLeaderboard();

        // 更新所有条目
        for (int i = 0; i < leaderboardEntries.Length; i++)
        {
            if (i < leaderboard.Count)
            {
                // 显示条目并更新数据
                leaderboardEntries[i].gameObject.SetActive(true);
                GameResult result = leaderboard[i];
                leaderboardEntries[i].SetData(
                    i + 1,
                    result.kills,
                    result.time,
                    result.timestamp
                );
            }
            else
            {
                // 隐藏多余的条目
                leaderboardEntries[i].gameObject.SetActive(false);
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        // 重置游戏状态
        GameManager.Instance.ResetGameState();

        // 重新加载场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // 确保玩家控制启用
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.EnablePlayerControl(true);
        }

        // 锁定光标
        UI_EscPancel.Instance?.LockCursor();
    }
}