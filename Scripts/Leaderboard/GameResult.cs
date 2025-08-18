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

    // �����ܷ֣�ֻ���ǻ�ɱ����ʱ�䣩
    public int CalculateScore()
    {
        // ��ɱ��Ȩ��: 100��ʱ��Ȩ��: -1��ʱ��Խ��Խ�ã�
        return kills * 100 + (int)(-time);
    }
}