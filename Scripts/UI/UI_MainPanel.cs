using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainPanel : MonoBehaviour
{
    public static UI_MainPanel Instance;
    public Text Hp_Text;
    public Text CurrBullet_Text;
    public Image CurrBullet_Image;
    public Text StandByBullet_Text;
    public Image StandByBullet_Image;
    
    public Slider Hp_Slider;
    public Image Hp_FillImage; // 血条填充图像

    // 颜色渐变设置
    public Gradient healthGradient;

    //击杀统计文本
    public Text KillCount_Text;
    private int killCount = 0;

    private void Awake()
    {
        Instance = this;
        // 初始化血条
        if (Hp_Slider != null)
        {
            Hp_Slider.maxValue = 100;
            Hp_Slider.minValue = 0;
            Hp_Slider.value = 100;

            // 获取Fill图像
            if (Hp_FillImage == null)
            {
                Hp_FillImage = Hp_Slider.fillRect.GetComponent<Image>();
            }

            // 应用初始颜色
            UpdateHealthColor(100);
            // 初始化击杀数显示
            UpdateKillCountText();
        }
    }
    // 根据血量更新血条颜色
    private void UpdateHealthColor(float currentHealth)
    {
        if (Hp_FillImage == null) return;

        // 计算血量百分比 (0-1)
        float healthPercentage = currentHealth / 100f;

        // 从渐变色中获取对应颜色
        Hp_FillImage.color = healthGradient.Evaluate(healthPercentage);
    }
    // 更新击杀数文本
    private void UpdateKillCountText()
    {
        if (KillCount_Text != null)
        {
            KillCount_Text.text = "已击杀: " + killCount.ToString();
        }
    }

    // 增加击杀数
    public void AddKill()
    {
        killCount++;
        UpdateKillCountText();
    }
    public int GetKillCount()
    {
        return killCount;
    }
    // 重置击杀数
    public void ResetKillCount()
    {
        killCount = 0;
        UpdateKillCountText();
    }
    public void UpdateHP_Text(int hp)
    {
        Hp_Text.text = hp.ToString();

        // 更新血条值
        if (Hp_Slider != null)
        {
            Hp_Slider.value = hp;
            UpdateHealthColor(hp);
        }

        // 文本颜色变化保持不变
        if (hp > 60)
        {
            Hp_Text.color = Color.green;
        }
        else if (hp > 30)
        {
            Hp_Text.color = Color.yellow;
        }
        else
        {
            Hp_Text.color = Color.red;
        }
    }
    public void UpdateCurrBullet_Text(int cur,int max)
    {
        CurrBullet_Text.text = cur + "/" + max;
        if (cur < 5)
        {
            CurrBullet_Text.color = Color.red;
        }
        else
        {
            CurrBullet_Text.color = Color.white;
        }
    }

    public void UpdateStandByBullet_Text(int num)
    {
        StandByBullet_Text.text = num.ToString();
        if (num < 30)
        {
            StandByBullet_Text.color = Color.red;
        }
        else
        {
            StandByBullet_Text.color = Color.white;
        }
    }

    /// <summary>
    /// 进入武器的初始化
    /// </summary>
    public void InitForEnterWeapon( bool wantBullet)
    {
        CurrBullet_Text.gameObject.SetActive(wantBullet);
        CurrBullet_Image.gameObject.SetActive(wantBullet);;
        StandByBullet_Text.gameObject.SetActive(wantBullet);
        StandByBullet_Image.gameObject.SetActive(wantBullet);
    }
}
