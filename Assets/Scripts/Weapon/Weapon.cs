using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public enum FireMode
    {
        Single,
        Brust,
        Auto
    }

    private Entity owner;
    public int weaponSelectIndex = 0;
    public bool selected { get; private set; } = false;

    [Header("��������")]
    [SerializeField] private int maxAmmo = 1;                                //���ϻ��ҩ��
    [SerializeField] private int maxPrepareAmmo = 16;                //���󱸵�ҩ��
    [SerializeField] private float shootingInterval = 0.1f;            //������
    [SerializeField] public float reloadTime = 2;                            //����ʱ��


    [Header("����ģʽ")]
    [SerializeField] public FireMode fireMode = FireMode.Single;      //����ģʽ 
    [SerializeField] private int brustNum = 3;                                      //brustһ�ο���������ӵ���
    [SerializeField] private float brustTime = 1;                                  //����brust����֮��ļ��


    //[Header("����״̬����")]
    private int curAmmo;                                    //��ǰ��ҩ��
    private int curPrepareAmmo;                       //��ǰ�󱸵�ҩ��
    private float shootingIntervalTimer = 0;    //��������ʱ��
    private float brustTimeTimer = 0;               //brust��������ʱ��
    private int brustCounter;                             //brust������


    [Header("������")]
    [SerializeField] public Transform fireSocket;           
    [SerializeField] private GameObject bulletPrefab;   
    public float bulletVelocity = 100;                              
    public float inertialVelocityMultipler = 10;               
    public bool constantSpeed = false;                             


    //����������ί�������ȥ�������벻��ֱ�Ӽ������������
    [Header("��ЧЧ��")]
    [SerializeField] private GameObject fireFX;
    private CinemachineImpulseSource impulseSource;
    public float cameraShakeMultipler = 1f;


    [Header("��Ƶ")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private float soundMultipler = 1f;


    //��ҵ������Ӧ��ί��ת��
    public UnityAction onFireStart, onFiring, onFireEnd, onReload;
    //����ʱ��ί�У���UIͨ��ʹ��
    public UnityAction onReloadStart, onReloadEnd;
    public UnityAction<float> onReloading;
    public UnityAction<int, int> onAmmoChanged;
    //�Ƿ�ѡ��
    public UnityAction<bool> onSelectChanged;

    //�Լ���״̬��
    private StateMachine stateMachine = new StateMachine();
    public WeaponIdleState idleState;
    public WeaponFireState fireState;
    public WeaponReloadState reloadState;

    private void Awake()
    {
        owner = GetComponent<Entity>();
        impulseSource = fireSocket.GetComponent<CinemachineImpulseSource>();

        if (weaponSelectIndex == 1)
            selected = true;

        //��ʼ������
        curAmmo = maxAmmo;
        curPrepareAmmo = maxPrepareAmmo;
        brustTimeTimer = brustTime;
        brustCounter = brustNum;

        //״̬��ʼ��
        idleState = new WeaponIdleState(stateMachine, this);
        fireState = new WeaponFireState(stateMachine, this);
        reloadState = new WeaponReloadState(stateMachine, this);

        //����ֵ��Controller���������������
        PlayerController.Ins.weapons[weaponSelectIndex] = this;
    }

    private void Start()
    {
        //��Ҳ�����Weapon
        owner.onFireStart += () => onFireStart?.Invoke();
        owner.onFireEnd += () => onFireEnd?.Invoke();
        owner.onReload += () => onReload?.Invoke();

        owner.onSelect += (num) =>
        {
            selected = num == weaponSelectIndex;
            onSelectChanged?.Invoke(selected);
        };

        owner.onAllSelect += () =>
        {
            selected = true;
            onSelectChanged?.Invoke(selected);
        };

        //��ʼ��״̬��
        stateMachine.Init(idleState);
    }

    private void Update()
    {
        stateMachine.Update();

        if (shootingIntervalTimer > 0)
            shootingIntervalTimer -= Time.deltaTime;

        if (brustCounter <= 0 && brustTimeTimer > 0)
        {
            brustTimeTimer -= Time.deltaTime;
            if (brustTimeTimer <= 0)
                brustCounter = brustNum;
        }

        if(owner.firing)
            onFiring?.Invoke();
    }

    public void ModifyAmmo(int amount)
    {
        curAmmo += amount;
        curAmmo = Mathf.Clamp(curAmmo, 0, maxAmmo);
        onAmmoChanged?.Invoke(curAmmo, curPrepareAmmo);
    }

    public void Fire()
    {
        if (!CanFire())
            return;

        //����Ͷ����
        Bullet bullet = Instantiate(bulletPrefab, fireSocket.position, fireSocket.rotation).GetComponent<Bullet>();
        bullet.Init(bulletVelocity, constantSpeed, new Vector3(owner.velocity.x, 0, owner.velocity.z) * inertialVelocityMultipler, owner.flag);

        //���ݸ���
        ModifyAmmo(-1);
        shootingIntervalTimer = shootingInterval;
        if (fireMode == FireMode.Brust)                 //�����Brustģʽ
        {
            brustCounter--;
            if (brustCounter <= 0)
                brustTimeTimer = brustTime;
        }

        //����ǹ����Ч
        Instantiate(fireFX, fireSocket.position, fireSocket.rotation);

        //�𶯣�
        impulseSource.m_DefaultVelocity.x = Random.Range(-1f, 1f) * cameraShakeMultipler;
        impulseSource.m_DefaultVelocity.y = Random.Range(-1f, 1f) * cameraShakeMultipler;
        impulseSource.m_DefaultVelocity.z = Random.Range(-1f, 1f) * cameraShakeMultipler;
        impulseSource.GenerateImpulse();

        //ǹ����Ч
        AudioManager.PlayClipAtPoint(fireSound, fireSocket.position, soundMultipler);
    }

    public void Reload()
    {
        int needAmmo = maxAmmo - curAmmo;
        curAmmo = Mathf.Min(curPrepareAmmo, maxAmmo);
        curPrepareAmmo = Mathf.Max(curPrepareAmmo - needAmmo, 0);
        onAmmoChanged?.Invoke(curAmmo, curPrepareAmmo);
        AudioManager.PlayClipAtPoint(reloadSound, owner.transform.position, soundMultipler);
    }

    public bool HaveAmmo() => curAmmo > 0;
    public bool HavePrepareAmmo() => curPrepareAmmo > 0;

    public bool CanFire()
    {
        if (!selected)
            return false;

        //û���ӵ�
        if (!HaveAmmo())
            return false;

        //û�н�����ȴ
        if (shootingIntervalTimer > 0)
            return false;

        //�����Brustģʽ
        //���CounterС�ڵ���0��˵�������ˣ����򲻹�
        //��������˲��һ�û��brust��ȴ���ǾͲ��ܴ�
        if (fireMode == FireMode.Brust && brustCounter <= 0 && brustTimeTimer > 0)
            return false;

        return true;
    }

    public bool CanBrust() => brustCounter > 0;

    public float GetReloadTime() => reloadTime;

    public int GetCurAmmo() => curAmmo;
    public int GetCurPrepareAmmo() => curPrepareAmmo;

    public bool CanReload() => curPrepareAmmo > 0 && curAmmo < maxAmmo;

    public State GetCurState() => stateMachine.curState;
}
