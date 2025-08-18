using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
///  玩家状态枚举
/// </summary>
public enum PlayerState
{
    Move,
    Shoot,
    Reload
}
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [SerializeField]FirstPersonController firstPersonController;//第一人称控制器

    //十字准星
    [SerializeField] Image crossImage;
    
    [SerializeField] Camera[] cameras; 
    #region 武器相关
    [SerializeField] WeaponBase[] weapons;      //玩家的武器列表
    private int currentWeaponIndex = -1;        //当前武器索引
    private int previousWeaponIndex = -1;       //上一个武器索引
    private bool canChangeWeapon = true;        //是否可以切换武器
    #endregion
    private int hp = 100;
    public PlayerState playerState;

    private bool playerControlEnabled = true;

    private void Awake()
    {
        Instance = this;
        //初始化所有武器
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Init(this);
        }
        playerState = PlayerState.Move;
        //默认选择第一个武器
        ChangeWeapon(0);
        LockCursor();
    }
    // 锁定光标的方法
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 启用/禁用玩家控制
    public void EnablePlayerControl(bool enable)
    {
        playerControlEnabled = enable;

        // 禁用第一人称控制器
        if (firstPersonController != null)
        {
            firstPersonController.enabled = enable;
        }
    }

    private void Update()
    {
        // 添加暂停检查 - 如果游戏暂停，跳过所有逻辑
        if (Mathf.Approximately(Time.timeScale, 0f)) return;
        //驱动武器层
        weapons[currentWeaponIndex].OnUpdatePlayerState(playerState);

        //按键检测切换武器
        if (canChangeWeapon == false) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (previousWeaponIndex >= 0) ChangeWeapon(previousWeaponIndex);
        }
    }

    /// <summary>
    /// 修改玩家状态
    /// </summary>
    public void ChangePlayerState(PlayerState newState)
    {
        playerState = newState;
        //武器在进入某个状态时，可能会有一些特定的逻辑
        weapons[currentWeaponIndex].OnEnterPlayerState(newState);
    }
    #region 后坐力

    /// <summary>
    /// 开始射击后坐力
    /// </summary>
    /// <param name="recoil">后坐力大小</param>
    public void StartShootRecoil(float recoil = 1)
    {
        StartCoroutine(ShootRecoil_Cross(recoil));
        //瞄准器放大缩小
        if (shootRecoil_CameraCoroutine != null)
        {
            StopCoroutine(shootRecoil_CameraCoroutine);
        }
        //视角移动
        StartCoroutine(ShootRecoil_Camera(recoil));
    }
    //后坐力-瞄准器
    IEnumerator ShootRecoil_Cross(float recoil)
    {
        Vector2 scale = crossImage.transform.localScale;
        //放大
        while (scale.x < 1.3f)
        {
            yield return null;
            scale.x += Time.deltaTime * 3 * recoil;
            scale.y += Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        //缩小
        while (scale.x > 1)
        {
            yield return null;
            scale.x -= Time.deltaTime * 3 * recoil;
            scale.y -= Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        crossImage.transform.localScale = Vector2.one;
    }

    private Coroutine shootRecoil_CameraCoroutine;
    IEnumerator ShootRecoil_Camera(float recoil)
    {
        float xOffset = Random.Range(0.3f, 0.6f) * recoil;
        float yOffset = Random.Range(-0.15f, 0.15f) * recoil;
        firstPersonController.xRotOffset = xOffset;
        firstPersonController.yRotOffset = yOffset;
        //让偏移发生六帧
        for (int i = 0; i < 6; i++)
        {
            yield return null;
        }
        //恢复偏移
        firstPersonController.xRotOffset = 0;
        firstPersonController.yRotOffset = 0;
    }
    #endregion

    public void Hurt(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
        UI_MainPanel.Instance.UpdateHP_Text(hp);

        //TODO:玩家死亡逻辑
        // 玩家死亡时触发游戏结束
        if (hp <= 0)
        {
            // 延迟0.5秒确保所有动画播放完毕
            Invoke("TriggerGameOver", 0.5f);
        }
    }
    private void TriggerGameOver()
    {
        GameManager.Instance.EndGame("玩家死亡");
    }
    private void ChangeWeapon(int newIndex)
    {
        //是不是重复在按同一个键
        if (currentWeaponIndex == newIndex)
        {
            return;
        }
        //上一个武器的索引 = 当前武器的索引
        previousWeaponIndex = currentWeaponIndex;
        //当前武器的索引 = 新的索引
        currentWeaponIndex = newIndex;
        
        //如果是第一次使用
        if (previousWeaponIndex < 0)
        {
            //直接进入当前武器
            weapons[currentWeaponIndex].Enter();

        }
        else
        {
            //要先退出这把武器
            //要等待上一把武器的退出完成后，才能播放新武器的动画
            weapons[previousWeaponIndex].Exit(OnWeaponExitOver);
            canChangeWeapon = false;
        }
    }
    private void OnWeaponExitOver()
    {
        canChangeWeapon = true;
        weapons[currentWeaponIndex].Enter();
    }

    /// <summary>
    /// 为武器做外部的初始化
    /// </summary>
    public void InitForOnEnterWeapon(bool wantCorsshair,bool wantBullet)
    {
        crossImage.gameObject.SetActive(wantCorsshair);
        UI_MainPanel.Instance.InitForEnterWeapon(wantBullet);
    }

    public void UpdateBullerUI(int curr_BullNum,int curr_MaxBulletNum,int standby_BulletNum)
    {
        UI_MainPanel.Instance.UpdateCurrBullet_Text(curr_BullNum, curr_MaxBulletNum);
        UI_MainPanel.Instance.UpdateStandByBullet_Text(standby_BulletNum);
    }
    public void SetCameraView(int value)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].fieldOfView = value;
        }
    }

    public int GetCurrentHealth()
    {
        return hp;
    }
}