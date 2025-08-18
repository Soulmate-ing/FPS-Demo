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
            EscPancel.SetActive(false); // ��ʼʱ����
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
        // ���Esc������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EscPancel.activeSelf)
            {
                ReturnGame(); // ����������ʾ���򷵻���Ϸ
            }
            else
            {
                OpenEscPancel(); // ��������
            }
        }
    }
    public void ReturnGame()
    {
        if (EscPancel != null)
        {
            EscPancel.SetActive(false);
            Time.timeScale = 1f; // �ָ���Ϸʱ��

            // ���������ع��
            LockCursor();

            // �ָ���ʱ��
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
            Time.timeScale = 0f; // ��ͣ��Ϸʱ��

            // ��������ʾ���
            UnlockCursor();

            // ��ͣ��ʱ��
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseTimer();
            }

            // ������ҿ���
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.EnablePlayerControl(false);
            }
        }
    }
    public void ReturnMenu()
    {
        // ������� - �˵���Ҫ�ɼ����
        UnlockCursor();

        // ����ʱ������
        Time.timeScale = 1f;

        // ȷ������ GameManager
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        // ���ز˵�����
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
