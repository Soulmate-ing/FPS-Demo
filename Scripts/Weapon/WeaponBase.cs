using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������������Ļ���
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    protected PlayerController player;

    #region ��ϻ��ص�����
    protected int curr_BulletNum;           //��ǰ�ӵ�����
    public int curr_MaxBulletNum;           //��ǰ�ӵ�����
    protected int standby_BulletNum;        //�����ӵ�����
    public int standby_MaxBulletNum;        //�����ӵ�����
    #endregion

    #region �������
    public int attackValue;             //������
    public int shootingDistance = 1500; //�������
    public bool wantCrosshair;          //�Ƿ���׼��
    public bool wantBullet;             //�Ƿ���Ҫ�ӵ�
    public bool wantShootEF;            //�Ƿ��������Ч
    public bool wantRecoil;             //�Ƿ��к�����
    public float recoilStrength;        //������ǿ��
    public bool canThroughWall;         //����Ƿ���Դ�͸ǽ��

    protected bool canShoot = false;        //�Ƿ�������
    private bool wantReloadOnEnter = false; //��������ʱ�Ƿ���Ҫ����
    #endregion

    #region Ч��
    [SerializeField] AudioClip[] audioClips;        //�õ���������Ч
    [SerializeField] protected  GameObject[] prefab_BulletEF;  //�ӵ�������ЧԤ����
    [SerializeField] GameObject shootEF;            //�����Ч
    #endregion

    public virtual void Init(PlayerController player)
    {
        this.player = player;
        //��ʼ���ӵ�
        curr_BulletNum = curr_MaxBulletNum;
        standby_BulletNum = standby_MaxBulletNum;
    }
    public abstract void OnEnterPlayerState(PlayerState playerState);
    public abstract void OnUpdatePlayerState(PlayerState playerState);
    protected bool IsGamePaused()
    {
        // ͨ��ʱ�������ж��Ƿ���ͣ
        return Mathf.Approximately(Time.timeScale, 0f);
    }
    public virtual void Enter()
    {
        player.SetCameraView(60);
        canShoot = false;
        //��������ʱ�ĳ�ʼ��(�Ƿ���Ҫ�ӵ���UI׼�ǵ�)
        player.InitForOnEnterWeapon(wantCrosshair,wantBullet);
        //�����ӵ�����
        if (wantBullet)
        {
            player.UpdateBullerUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
            if (curr_BulletNum > 0)
            {
                //PlayAudio(0);
            }
            else
            {
                //�����ʱ���Ҫ�����ӵ�
                wantReloadOnEnter = true;
            }
        }
        //����һЩ״��
        if (shootEF != null )
        {
            shootEF.SetActive(false);
        }
        gameObject.SetActive(true);
    }
    private Action onExitOver;
    /// <summary>
    /// �˳�����
    /// </summary>
    public virtual void Exit(Action onExitOver)
    {
        animator.SetTrigger("Exit");
        this.onExitOver = onExitOver;
        player.ChangePlayerState(PlayerState.Move);
    }

    protected virtual void OnLeftAttack()
    {
        //�����ӵ�
        if (wantBullet)
        {
            curr_BulletNum--;
            player.UpdateBullerUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        }
        canShoot = false;
        //�����������
        animator.SetTrigger("Shoot");
        //��
        if (wantShootEF) shootEF.gameObject.SetActive(true);
        //������
        if (wantRecoil) player.StartShootRecoil(recoilStrength);
        //��Ч
        PlayAudio(1);
        

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //��ǽ
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
            //�˺��ж�
            if (Physics.Raycast(ray, out RaycastHit hitInfo, shootingDistance))
            {
                HitGameObject(hitInfo);
            }
        }
    }
    private void HitGameObject(RaycastHit hitInfo)
    {
        //�ж��ǲ��ǹ������˽�ʬ
        if(hitInfo.collider.gameObject.CompareTag("Zombie"))
        {
            //���н�ʬЧ��
            GameObject go = Instantiate(prefab_BulletEF[1], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
            //��ʬ���߼�
            ZombieController zombie = hitInfo.collider.GetComponent<ZombieController>();
            if (zombie == null)
            {
                zombie = hitInfo.collider.GetComponentInParent<ZombieController>();
            }
            zombie.Hurt(attackValue);
        }
        else if (hitInfo.collider.gameObject != player.gameObject)
        {
            //����Ч��
            GameObject go = Instantiate(prefab_BulletEF[0], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }
    }
    protected void PlayAudio(int index)
    {
        audioSource.PlayOneShot(audioClips[index]);
    }
    #region �����¼�
    private void EnterOver()
    {
        canShoot = true;
        //Debug.Log($"���� {gameObject.name} ���붯����ɣ������״̬: {canShoot}");
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
        //����ӵ�
        int want = curr_MaxBulletNum - curr_BulletNum;
        if(standby_BulletNum - want <0)
        {
            want = standby_BulletNum;
        }
        standby_BulletNum -= want;
        curr_BulletNum += want;
        //����UI
        player.UpdateBullerUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        animator.SetBool("Reload", false);
        player.ChangePlayerState(PlayerState.Move);
    }
    #endregion
}
