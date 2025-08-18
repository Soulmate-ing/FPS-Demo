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

    // ֱ�����ó����е�10����Ŀ
    public LeaderboardEntryUI[] leaderboardEntries = new LeaderboardEntryUI[10];
    private void Awake()
    {
        Instance = this;
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(string reason, int kills, float time)
    {
        gameOverPanel.SetActive(true);

        // ��ʾ������Ϸ���
        resultText.text = reason;
        killsText.text = $"Kill: {kills}";
        timeText.text = $"Time: {FormatTime(time)}";

        // �������а�
        PopulateLeaderboard();
    }

    public void PopulateLeaderboard()
    {
        // ��ȡ���а�����
        List<GameResult> leaderboard = LeaderboardManager.Instance.GetLeaderboard();

        // ����������Ŀ
        for (int i = 0; i < leaderboardEntries.Length; i++)
        {
            if (i < leaderboard.Count)
            {
                // ��ʾ��Ŀ����������
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
                // ���ض������Ŀ
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

        // ������Ϸ״̬
        GameManager.Instance.ResetGameState();

        // ���¼��س���
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // ȷ����ҿ�������
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.EnablePlayerControl(true);
        }

        // �������
        UI_EscPancel.Instance?.LockCursor();
    }
}