using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI_EscPancel : MonoBehaviour
{
    public static UI_EscPancel Instance;
    public GameObject EscPancel;
    public GameObject TipImage;
    private void Awake()
    {
        Instance = this;
        if (EscPancel != null)
        {
            EscPancel.SetActive(false); // 初始时隐藏
        }
        LockCursor();
    }
    public void LockCursor()
    {
        UnityEngine.Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    private void Update()
    {
        // 检测Esc键按下
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EscPancel.activeSelf)
            {
                ReturnGame(); // 如果面板已显示，则返回游戏
            }
            else
            {
                OpenEscPancel(); // 否则打开面板
            }
        }
    }
    public void ReturnGame()
    {
        if (EscPancel != null)
        {
            EscPancel.SetActive(false);
            Time.timeScale = 1f; // 恢复游戏时间

            // 锁定并隐藏光标
            LockCursor();

            // 恢复计时器
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartTimer();
            }

            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.EnablePlayerControl(true);
            }
        }
    }
    public void OpenEscPancel()
    {
        if (EscPancel != null)
        {
            EscPancel.SetActive(true);
            Time.timeScale = 0f; // 暂停游戏时间

            // 解锁并显示光标
            UnlockCursor();

            // 暂停计时器
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseTimer();
            }

            // 禁用玩家控制
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.EnablePlayerControl(false);
            }
        }
    }
    public void ReturnMenu()
    {
        // 解锁光标 - 菜单需要可见光标
        UnlockCursor();

        // 重置时间缩放
        Time.timeScale = 1f;

        // 确保销毁 GameManager
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        // 加载菜单场景
        SceneManager.LoadScene("Menu");
    }
    public void OpenTipImage()
    {
        if (TipImage != null)
        {
            TipImage.SetActive(true);
        }
    }
    public void CloseTipImage()
    {
        if (TipImage != null)
        {
            TipImage.SetActive(false);
        }
    }
}
