using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : WeaponBase
{
    [SerializeField] Knife_Collider knife_Collider;
    private bool isLeftAttack;
    public override void Init(PlayerController player)
    {
        base.Init(player);
        knife_Collider.Init(this);
    }
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot: 
                if (isLeftAttack) OnLeftAttack();
                else OnRightAttack();
                break;
        }
    }

    public override void OnUpdatePlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Move:
                if (IsGamePaused()) return;
                //�������
                if (canShoot && Input.GetMouseButton(0))
                {
                    isLeftAttack = true;
                    player.ChangePlayerState(PlayerState.Shoot);
                    return;
                }
                if (canShoot && Input.GetMouseButton(1))
                {
                    isLeftAttack = false;
                    player.ChangePlayerState(PlayerState.Shoot);
                    return;
                }
                break;
        }
    }
    public void HitTarget(GameObject hitObj,Vector3 efPos)
    {
        if (hitObj == player.gameObject)
            return;

        PlayAudio(2);
        //�ж��ǲ��ǹ������˽�ʬ
        if (hitObj.CompareTag("Zombie"))
        {
            //���н�ʬЧ��
            GameObject go = Instantiate(prefab_BulletEF[1], efPos, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
            //��ʬ���߼�
            ZombieController zombie = hitObj.GetComponent<ZombieController>();
            if (zombie == null)
            {
                zombie = hitObj.GetComponentInParent<ZombieController>();
            }
            zombie.Hurt(attackValue);
        }
        else if (hitObj != player.gameObject)
        {
            ////����Ч��
            //GameObject go = Instantiate(prefab_BulletEF[0], efPos, Quaternion.identity);
            //go.transform.LookAt(Camera.main.transform);
        }
    }
    protected override void OnLeftAttack()
    {
        PlayAudio(1);
        animator.SetTrigger("Shoot");
        animator.SetBool("IsLeft", true);
        knife_Collider.StartHit();
    }
    private void OnRightAttack()
    {
        PlayAudio(1);
        animator.SetTrigger("Shoot");
        animator.SetBool("IsLeft", false);
        knife_Collider.StartHit();
    }
    protected override void ShootOver()
    {
        base.ShootOver();
        knife_Collider.StopHit();
    }
}
