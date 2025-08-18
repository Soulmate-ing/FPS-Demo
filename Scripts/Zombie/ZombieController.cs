using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    Idle,
    Walk,
    Run,
    Attack,
    Hurt,
    Dead
}

public class ZombieController : MonoBehaviour
{
    [SerializeField]
    private ZombieState zombieState;
    private NavMeshAgent navMeshAgent;
    private AudioSource audioSource;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    public Zombie_Weapon weapon;

    private int hp = 100;
    public AudioClip[] FootstepAudioClips;  // 行走的音效
    public AudioClip[] IdelAudioClips;      // 待机的音效
    public AudioClip[] HurtAudioClips;      // 受伤的音效
    public AudioClip[] AttackAudioClips;    // 攻击的音效

    private Vector3 target;

    // 行走随机性参数
    private float walkChangeInterval = 5f;   // 改变方向的间隔时间
    private float walkChangeTimer = 0f;      // 改变方向的计时器
    private float maxWalkDistance = 15f;     // 最大行走距离

    // 状态切换时的逻辑
    public ZombieState ZombieState
    {
        get => zombieState;
        set
        {
            if (zombieState == ZombieState.Dead && value != ZombieState.Idle)
            {
                return;
            }
            zombieState = value;

            switch (zombieState)
            {
                case ZombieState.Idle:
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    navMeshAgent.enabled = false;
                    Invoke("GoWalk", Random.Range(1, 3)); // 等待1-3秒后走
                    break;
                case ZombieState.Walk:
                    animator.SetBool("Walk", true);
                    animator.SetBool("Run", false);
                    navMeshAgent.enabled = true;
                    navMeshAgent.speed = Random.Range(0.3f, 0.7f); // 随机速度

                    // 设置随机目标点
                    target = GetRandomWalkTarget();
                    navMeshAgent.SetDestination(target);
                    break;
                case ZombieState.Attack:
                    navMeshAgent.enabled = true;
                    // 停止移动并禁止自动旋转
                    navMeshAgent.isStopped = true;
                    navMeshAgent.updateRotation = false;
                    // 立即转向玩家
                    transform.LookAt(PlayerController.Instance.transform);
                    animator.SetTrigger("Attack");
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    break;
                case ZombieState.Run:
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", true);
                    navMeshAgent.enabled = true;
                    navMeshAgent.speed = 4f;
                    // 恢复移动和旋转
                    navMeshAgent.isStopped = false;
                    navMeshAgent.updateRotation = true;
                    break;
                case ZombieState.Hurt:
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetTrigger("Hurt");
                    break;
                case ZombieState.Dead:
                    if (UI_MainPanel.Instance != null)
                    {
                        UI_MainPanel.Instance.AddKill();
                    }
                    navMeshAgent.enabled = false;
                    animator.SetTrigger("Dead");
                    capsuleCollider.enabled = false;
                    Invoke("Destroy", 3);
                    break;
                default:
                    break;
            }
        }
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        weapon.Init(this);
        ZombieState = ZombieState.Idle;

        // 初始化随机计时器
        walkChangeTimer = Random.Range(0, walkChangeInterval);
    }

    // 处理脏数据
    public void Init()
    {
        animator.SetTrigger("Init");
        capsuleCollider.enabled = true;
        hp = 100;
        ZombieState = ZombieState.Idle;
    }

    void Update()
    {
        StateForUpdate();
    }

    void StateForUpdate()
    {
        float dis = 10f; // 发现玩家的距离

        switch (zombieState)
        {
            case ZombieState.Idle:
                break;

            case ZombieState.Walk:
                // 随机改变方向逻辑
                walkChangeTimer += Time.deltaTime;
                if (walkChangeTimer >= walkChangeInterval)
                {
                    // 50%几率改变方向
                    if (Random.value > 0.5f)
                    {
                        target = GetRandomWalkTarget();
                        navMeshAgent.SetDestination(target);
                    }
                    walkChangeTimer = 0;
                    walkChangeInterval = Random.Range(3f, 8f); // 随机间隔时间
                }

                // 如果接近目标点，重新设置目标
                if (Vector3.Distance(transform.position, target) <= 1f)
                {
                    target = GetRandomWalkTarget();
                    navMeshAgent.SetDestination(target);
                }

                // 如果发现玩家，进入追击状态
                if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < dis)
                {
                    ZombieState = ZombieState.Run;
                    return;
                }
                break;

            case ZombieState.Run:
                // 一直追玩家
                navMeshAgent.SetDestination(PlayerController.Instance.transform.position);
                if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 3f)
                {
                    ZombieState = ZombieState.Attack;
                }
                break;

            case ZombieState.Attack:
                // 攻击时持续面向玩家
                if (PlayerController.Instance != null)
                {
                    Vector3 direction = PlayerController.Instance.transform.position - transform.position;
                    direction.y = 0; // 保持水平旋转
                    if (direction != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            Quaternion.LookRotation(direction),
                            Time.deltaTime * 10f
                        );
                    }
                }

                // 如果攻击动画播放完成，返回追击状态
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 &&
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    ZombieState = ZombieState.Run;
                }
                break;


            case ZombieState.Hurt:
                break;

            case ZombieState.Dead:
                break;

            default:
                break;
        }
    }

    // 获取随机行走目标点
    private Vector3 GetRandomWalkTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * maxWalkDistance;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, maxWalkDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // 如果找不到有效点，返回当前位置
        return transform.position;
    }

    void GoWalk() // 动画状态机调用
    {
        ZombieState = ZombieState.Walk;
    }

    public void Hurt(int value)
    {
        hp -= value;
        if (hp <= 0)
        {
            ZombieState = ZombieState.Dead;
        }
        else
        {
            // 击退
            StartCoroutine(MovePause());
        }
    }

    void Destroy() // 动画状态机调用
    {
        ZombieManager.Instance.ZombieDead(this);
    }

    IEnumerator MovePause()
    {
        ZombieState = ZombieState.Hurt;
        navMeshAgent.enabled = false;
        yield return new WaitForSeconds(0.5f);
        if (ZombieState != ZombieState.Run)
        {
            ZombieState = ZombieState.Run;
        }
    }

    #region 动画事件
    void IdelAudio()
    {
        if (Random.Range(0, 4) == 1)
        {
            audioSource.PlayOneShot(IdelAudioClips[Random.Range(0, IdelAudioClips.Length)]);
        }
    }

    void FootStep()
    {
        audioSource.PlayOneShot(FootstepAudioClips[Random.Range(0, FootstepAudioClips.Length)]);
    }

    private void HurtAudio()
    {
        audioSource.PlayOneShot(HurtAudioClips[Random.Range(0, HurtAudioClips.Length)]);
    }

    private void AttackAudio()
    {
        audioSource.PlayOneShot(AttackAudioClips[Random.Range(0, AttackAudioClips.Length)]);
    }

    public void StartAttack()
    {
        weapon.StartAttack();
    }

    public void EndAttack()
    {
        weapon.EndAttack();
    }
    #endregion
}