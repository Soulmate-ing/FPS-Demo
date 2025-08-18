using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Weapon : MonoBehaviour
{
    private ZombieController zombie;
    private BoxCollider boxCollider;

    public void Init(ZombieController zombie)
    {
        this.zombie = zombie;
        boxCollider = GetComponent<BoxCollider>();
    }
    public void StartAttack()
    {
        isAttacked = false;
        boxCollider.enabled = true;
    }
    public void EndAttack()
    {
        boxCollider.enabled = false;
    }
    private bool isAttacked = false;
    private void OnTriggerEnter(Collider other)
    {
        //当武器碰撞到玩家，并且这次攻击还没造成过伤害时，才执行一次伤害逻辑
        if (!isAttacked && other.gameObject.tag == "Player")
        {
            isAttacked = true;
            PlayerController.Instance.Hurt(10);
        }
    }
}
