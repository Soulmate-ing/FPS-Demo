using UnityEngine;
using UnityEngine.UI;
using System;

public class LeaderboardEntryUI : MonoBehaviour
{
    public Text rankText;
    public Text killsText;
    public Text timeText;
    public Text dateText; // 添加日期文本

    public void SetData(int rank, int kills, float time, DateTime date)
    {
        // 设置排名文本（带奖牌图标）
        rankText.text = $"{GetRankIcon(rank)} {rank}.";
        killsText.text = kills.ToString();
        timeText.text = FormatTime(time);

        // 设置背景色交替
        GetComponent<Image>().color = rank % 2 == 0 ?
            new Color(0.2f, 0.2f, 0.2f, 0.5f) :
            new Color(0.3f, 0.3f, 0.3f, 0.5f);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    private string GetRankIcon(int rank)
    {
        switch (rank)
        {
            case 1: return "🥇";
            case 2: return "🥈";
            case 3: return "🥉";
            default: return "";
        }
    }
}