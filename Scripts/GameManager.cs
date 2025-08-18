using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 添加UI命名空间

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform[] Points;

    // 计时器相关变量
    [Header("计时器设置")]
    public Text timerText; // UI文本组件，显示计时器
    public bool countUp = true; // true:正计时 false:倒计时
    public float initialTime = 0f; // 初始时间（正计时时为0，倒计时时为设置值）
    public float timeLimit = 300f; // 倒计时时间限制（秒）
    public bool autoStart = true; // 游戏开始时自动启动计时器

    private float currentTime; // 当前时间
    private bool isTimerRunning = false;
    private bool gameEnded = false;
    private void Awake()
    {
        // 确保单例唯一性
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 如果不需要跨场景，移除 DontDestroyOnLoad
        // DontDestroyOnLoad(gameObject);

        // 初始化计时器
        ResetGameState();
    }
    // 添加重置游戏状态的方法
    public void ResetGameState()
    {
        gameEnded = false;
        isTimerRunning = false;
        currentTime = countUp ? initialTime : timeLimit;
        UpdateTimerDisplay();

        if (autoStart)
        {
            StartTimer();
        }
    }
    void Start()
    {
        FindTimerText(); // 确保获取引用

        // 初始化计时器
        currentTime = countUp ? initialTime : timeLimit;
        UpdateTimerDisplay();

        if (autoStart)
        {
            StartTimer();
        }
    }
    private void FindTimerText()
    {
        if (timerText == null)
        {
            GameObject timerObj = GameObject.Find("TimerText");
            if (timerObj != null)
            {
                timerText = timerObj.GetComponent<Text>();
            }
        }
    }
    void Update()
    {
        if (isTimerRunning)
        {
            // 更新计时器
            if (countUp)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime -= Time.deltaTime;

                // 检查倒计时结束
                if (currentTime <= 0)
                {
                    currentTime = 0;
                    EndGame("时间结束！");
                }
            }

            UpdateTimerDisplay();
        }
    }

    // 更新UI显示
    private void UpdateTimerDisplay()
    {
        FindTimerText(); // 双重确保

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    // 开始计时器
    public void StartTimer()
    {
        isTimerRunning = true;
    }

    // 暂停计时器
    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    // 重置计时器
    public void ResetTimer()
    {
        ResetGameState();
    }

    // 获取当前时间
    public float GetCurrentTime()
    {
        return currentTime;
    }

    // 游戏结束方法
    public void EndGame(string reason)
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;
        UI_EscPancel.Instance?.UnlockCursor();

        // 只获取击杀数和时间
        int kills = UI_MainPanel.Instance.GetKillCount();
        float time = GetCurrentTime();

        // 添加结果（不再传递血量）
        LeaderboardManager.Instance.AddResult(kills, time);

        // 显示游戏结束UI（不再传递血量）
        GameOverUI.Instance.ShowGameOver(reason, kills, time);
    }
    public Vector3 GetPoints()
    {
        return Points[Random.Range(0, Points.Length)].position;
    }
}