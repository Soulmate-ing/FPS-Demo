using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : WeaponBase
{
    [SerializeField] GameObject sightCanvas;    //瞄准镜UI
    [SerializeField] GameObject[] renders;
    private bool isAim = false;                 //是否开镜
    public override void Enter()
    {
        if (isAim) StopAim();
        isAim = false;
        base.Enter();
    }
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot:
                if (isAim) StopAim();
                isAim = false;
                OnLeftAttack();
                break;
            case PlayerState.Reload:
                if (isAim) StopAim();
                isAim = false;
                PlayAudio(2);
                animator.SetBool("Reload", true);
                break;
        }
    }

    public override void OnUpdatePlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Move:
                if (IsGamePaused()) return;
                //有可能需要切换子弹
                //第一种情况：子弹打完了 但是要有备用子弹才行
                if (curr_BulletNum == 0 && standby_BulletNum > 0)
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                //第二种情况：子弹没打完 但是玩家按了换弹键R
                if (Input.GetKeyDown(KeyCode.R) && standby_BulletNum > 0)
                {
                    player.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                //有可能要射击
                //当前没有在换子弹中
                //当前弹匣里面有子弹
                //按鼠标左键
                if (canShoot && curr_BulletNum > 0 && Input.GetMouseButton(0))
                {
                    player.ChangePlayerState(PlayerState.Shoot);
                }

                //开镜/关镜
                if(canShoot && Input.GetMouseButtonDown(1))
                {
                    isAim = !isAim;
                    if(isAim) StartAim();
                    else StopAim();
                }
                break;
        }
    }
    private void StartAim()
    {
        //播放动画
        animator.SetBool("Aim", true);
        //关闭火花效果
        wantShootEF = false; 
    }
    private void StopAim()
    {
        animator.SetBool("Aim", false);
        wantShootEF = true;
        //显示所有的渲染器
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].SetActive(true);
        }
        //关闭狙击镜
        sightCanvas.SetActive(false);
        //设置镜头缩放
        player.SetCameraView(60);
    }
    #region 动画事件
    private void StartLoad()
    {
        PlayAudio(3); 
    }
    private void AimOver()
    {
        
        StartCoroutine(DoAim());
    }
    IEnumerator DoAim()
    {
        //隐藏所有的渲染器
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].SetActive(false);
        }
        //停留一点时间
        yield return new WaitForSeconds(0.1f);
        //显示狙击镜
        sightCanvas.SetActive(true);
        //设置镜头缩放
        player.SetCameraView(30);
    }
    #endregion
}
