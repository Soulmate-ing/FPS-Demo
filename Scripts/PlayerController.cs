using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
///  ���״̬ö��
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
    [SerializeField]FirstPersonController firstPersonController;//��һ�˳ƿ�����

    //ʮ��׼��
    [SerializeField] Image crossImage;
    
    [SerializeField] Camera[] cameras; 
    #region �������
    [SerializeField] WeaponBase[] weapons;      //��ҵ������б�
    private int currentWeaponIndex = -1;        //��ǰ��������
    private int previousWeaponIndex = -1;       //��һ����������
    private bool canChangeWeapon = true;        //�Ƿ�����л�����
    #endregion
    private int hp = 100;
    public PlayerState playerState;

    private bool playerControlEnabled = true;

    private void Awake()
    {
        Instance = this;
        //��ʼ����������
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Init(this);
        }
        playerState = PlayerState.Move;
        //Ĭ��ѡ���һ������
        ChangeWeapon(0);
        LockCursor();
    }
    // �������ķ���
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ����/������ҿ���
    public void EnablePlayerControl(bool enable)
    {
        playerControlEnabled = enable;

        // ���õ�һ�˳ƿ�����
        if (firstPersonController != null)
        {
            firstPersonController.enabled = enable;
        }
    }

    private void Update()
    {
        // �����ͣ��� - �����Ϸ��ͣ�����������߼�
        if (Mathf.Approximately(Time.timeScale, 0f)) return;
        //����������
        weapons[currentWeaponIndex].OnUpdatePlayerState(playerState);

        //��������л�����
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
    /// �޸����״̬
    /// </summary>
    public void ChangePlayerState(PlayerState newState)
    {
        playerState = newState;
        //�����ڽ���ĳ��״̬ʱ�����ܻ���һЩ�ض����߼�
        weapons[currentWeaponIndex].OnEnterPlayerState(newState);
    }
    #region ������

    /// <summary>
    /// ��ʼ���������
    /// </summary>
    /// <param name="recoil">��������С</param>
    public void StartShootRecoil(float recoil = 1)
    {
        StartCoroutine(ShootRecoil_Cross(recoil));
        //��׼���Ŵ���С
        if (shootRecoil_CameraCoroutine != null)
        {
            StopCoroutine(shootRecoil_CameraCoroutine);
        }
        //�ӽ��ƶ�
        StartCoroutine(ShootRecoil_Camera(recoil));
    }
    //������-��׼��
    IEnumerator ShootRecoil_Cross(float recoil)
    {
        Vector2 scale = crossImage.transform.localScale;
        //�Ŵ�
        while (scale.x < 1.3f)
        {
            yield return null;
            scale.x += Time.deltaTime * 3 * recoil;
            scale.y += Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        //��С
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
        //��ƫ�Ʒ�����֡
        for (int i = 0; i < 6; i++)
        {
            yield return null;
        }
        //�ָ�ƫ��
        firstPersonController.xRotOffset = 0;
        firstPersonController.yRotOffset = 0;
    }
    #endregion

    public void Hurt(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
        UI_MainPanel.Instance.UpdateHP_Text(hp);

        //TODO:��������߼�
        // �������ʱ������Ϸ����
        if (hp <= 0)
        {
            // �ӳ�0.5��ȷ�����ж����������
            Invoke("TriggerGameOver", 0.5f);
        }
    }
    private void TriggerGameOver()
    {
        GameManager.Instance.EndGame("�������");
    }
    private void ChangeWeapon(int newIndex)
    {
        //�ǲ����ظ��ڰ�ͬһ����
        if (currentWeaponIndex == newIndex)
        {
            return;
        }
        //��һ������������ = ��ǰ����������
        previousWeaponIndex = currentWeaponIndex;
        //��ǰ���������� = �µ�����
        currentWeaponIndex = newIndex;
        
        //����ǵ�һ��ʹ��
        if (previousWeaponIndex < 0)
        {
            //ֱ�ӽ��뵱ǰ����
            weapons[currentWeaponIndex].Enter();

        }
        else
        {
            //Ҫ���˳��������
            //Ҫ�ȴ���һ���������˳���ɺ󣬲��ܲ����������Ķ���
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
    /// Ϊ�������ⲿ�ĳ�ʼ��
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