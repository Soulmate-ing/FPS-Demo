using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    public static ZombieManager Instance;
    public GameObject prefab_Zombie;
    public int zombieNums;

    public List<ZombieController> zombies; // 当前场景中的僵尸
    private Queue<ZombieController> zombiePool = new Queue<ZombieController>(); // 备用僵尸
    public Transform Pool;

    public int batchSize = 5; // 每批生成的数量
    public float batchInterval = 0.1f; // 批次间的间隔时间
    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        // 初始生成所有僵尸
        StartCoroutine(InitializeZombies());
        // 开始检查僵尸数量
        StartCoroutine(CheckZombie());
    }

    // 初始生成所有僵尸（按批次生成）
    IEnumerator InitializeZombies()
    {
        // 等待GameManager初始化完成
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        int zombiesCreated = 0;

        while (zombiesCreated < zombieNums)
        {
            // 计算本次要生成的数量
            int createCount = Mathf.Min(batchSize, zombieNums - zombiesCreated);

            // 一次性生成一批僵尸
            for (int i = 0; i < createCount; i++)
            {
                // 创建僵尸
                GameObject zb = Instantiate(
                    prefab_Zombie,
                    GameManager.Instance.GetPoints(),
                    Quaternion.identity,
                    transform
                );

                ZombieController zombie = zb.GetComponent<ZombieController>();
                zombies.Add(zombie);
            }

            zombiesCreated += createCount;

            // 等待一段时间再生成下一批
            yield return new WaitForSeconds(batchInterval);
        }
    }

    // 检查僵尸
    IEnumerator CheckZombie()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            // 僵尸数量不够，产生僵尸
            if (zombies.Count < zombieNums)
            {
                // 池子里面有，从池子拿
                if (zombiePool.Count > 0)
                {
                    ZombieController zb = zombiePool.Dequeue();
                    zb.transform.SetParent(transform);
                    zb.transform.position = GameManager.Instance.GetPoints();
                    zombies.Add(zb);
                    zb.gameObject.SetActive(true);
                    zb.Init();
                    yield return new WaitForSeconds(2);
                }
                // 池子没有，就实例化
                else
                {
                    GameObject zb = Instantiate(prefab_Zombie, GameManager.Instance.GetPoints(), Quaternion.identity, transform);
                    zombies.Add(zb.GetComponent<ZombieController>());
                }
            }
        }
    }

    public void ZombieDead(ZombieController zombie)
    {
        zombies.Remove(zombie);
        zombiePool.Enqueue(zombie);
        zombie.gameObject.SetActive(false);
        zombie.transform.SetParent(Pool);
    }
}