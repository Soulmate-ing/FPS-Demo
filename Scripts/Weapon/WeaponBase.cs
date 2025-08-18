using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有玩家武器的基类
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    protected PlayerController player;

    #region 弹匣相关的数据
    protected int curr_BulletNum;           //当前子弹数量
    public int curr_MaxBulletNum;           //当前子弹上限
    protected int standby_BulletNum;        //备用子弹数量
    public int standby_MaxBulletNum;        //备用子弹上限
    #endregion

    #region 射击参数
    public int attackValue;             //攻击力
    public int shootingDistance = 1500; //射击距离
    public bool wantCrosshair;          //是否有准星
    public bool wantBullet;             //是否需要子弹
    public bool wantShootEF;            //是否有射击特效
    public bool wantRecoil;             //是否有后坐力
    public float recoilStrength;        //后坐力强度
    public bool canThroughWall;         //射击是否可以穿透墙壁

    protected bool canShoot = false;        //是否可以射击
    private bool wantReloadOnEnter = false; //进入武器时是否需要换弹
    #endregion

    #region 效果
    [SerializeField] AudioClip[] audioClips;        //用到的所有音效
    [SerializeField] protected  GameObject[] prefab_BulletEF;  //子弹命中特效预制体
    [SerializeField] GameObject shootEF;            //射击特效
    #endregion

    public virtual void Init(PlayerController player)
    {
        this.player = player;
        //初始化子弹
        curr_BulletNum = curr_MaxBulletNum;
        standby_BulletNum = standby_MaxBulletNum;
    }
    public abstract void OnEnterPlayerState(PlayerState playerState);
    public abstract void OnUpdatePlayerState(PlayerState playerState);
    protected bool IsGamePaused()
    {
        // 通过时间缩放判断是否暂停
        return Mathf.Approximately(Time.timeScale, 0f);
    }
    public virtual void Enter()
    {
        player.SetCameraView(60);
        canShoot = false;
        //进入武器时的初始化(是否需要子弹、UI准星等)
        player.InitForOnEnterWeapon(wantCrosshair,wantBullet);
        //更新子弹数量
        if (wantBullet)
        {
            player.UpdateBullerUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
            if (curr_BulletNum > 0)
            {
                //PlayAudio(0);
            }
            else
            {
                //进入的时候就要更换子弹
                wantReloadOnEnter = true;
            }
        }
        //重置一些状况
        if (shootEF != null )
        {
            shootEF.SetActive(false);
        }
        gameObject.SetActive(true);
    }
    private Action onExitOver;
    /// <summary>
    /// 退出武器
    /// </summary>
    public virtual void Exit(Action onExitOver)
    {
        animator.SetTrigger("Exit");
        this.onExitOver = onExitOver;
        player.ChangePlayerState(PlayerState.Move);
    }

    protected virtual void OnLeftAttack()
    {
        //更新子弹
        if (wantBullet)
        {
            curr_BulletNum--;
            player.UpdateBullerUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        }
        canShoot = false;
        //播放射击动画
        animator.SetTrigger("Shoot");
        //火花
        if (wantShootEF) shootEF.gameObject.SetActive(true);
        //后坐力
        if (wantRecoil) player.StartShootRecoil(recoilStrength);
        //音效
        PlayAudio(1);
        

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //穿墙
        if (canThroughWall)
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, shootingDistance);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                HitGameObject(raycastHits[i]);
            }
        }
        else
        {
            //伤害判定
            if (Physics.Raycast(ray, out RaycastHit hitInfo, shootingDistance))
            {
                HitGameObject(hitInfo);
            }
        }
    }
    private void HitGameObject(RaycastHit hitInfo)
    {
        //判断是不是攻击到了僵尸
        if(hitInfo.collider.gameObject.CompareTag("Zombie"))
        {
            //命中僵尸效果
            GameObject go = Instantiate(prefab_BulletEF[1], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
            //僵尸的逻辑
            ZombieController zombie = hitInfo.collider.GetComponent<ZombieController>();
            if (zombie == null)
            {
                zombie = hitInfo.collider.GetComponentInParent<ZombieController>();
            }
            zombie.Hurt(attackValue);
        }
        else if (hitInfo.collider.gameObject != player.gameObject)
        {
            //命中效果
            GameObject go = Instantiate(prefab_BulletEF[0], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }
    }
    protected void PlayAudio(int index)
    {
        audioSource.PlayOneShot(audioClips[index]);
    }
    #region 动画事件
    private void EnterOver()
    {
        canShoot = true;
        //Debug.Log($"武器 {gameObject.name} 进入动画完成，可射击状态: {canShoot}");
        if (wantReloadOnEnter)
        {
            player.ChangePlayerState(PlayerState.Reload);
        }
    }
    private void ExitOver()
    {
        gameObject.SetActive(false);
        onExitOver?.Invoke();
    }
    protected virtual void ShootOver()
    {
        canShoot = true;
        if (wantShootEF) shootEF.SetActive(false); 
        if(player.playerState == PlayerState.Shoot)
        {
            player.ChangePlayerState(PlayerState.Move);
        }
    }
    private void ReloadOver()
    {
        //填充子弹
        int want = curr_MaxBulletNum - curr_BulletNum;
        if(standby_BulletNum - want <0)
        {
            want = standby_BulletNum;
        }
        standby_BulletNum -= want;
        curr_BulletNum += want;
        //更新UI
        player.UpdateBullerUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        animator.SetBool("Reload", false);
        player.ChangePlayerState(PlayerState.Move);
    }
    #endregion
}
