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

    [Header("武器性能")]
    [SerializeField] private int maxAmmo = 1;                                //最大弹匣弹药量
    [SerializeField] private int maxPrepareAmmo = 16;                //最大后备弹药量
    [SerializeField] private float shootingInterval = 0.1f;            //射击间隔
    [SerializeField] public float reloadTime = 2;                            //换弹时间


    [Header("开火模式")]
    [SerializeField] public FireMode fireMode = FireMode.Single;      //开火模式 
    [SerializeField] private int brustNum = 3;                                      //brust一次开火射出的子弹数
    [SerializeField] private float brustTime = 1;                                  //两次brust开火之间的间隔


    //[Header("武器状态参数")]
    private int curAmmo;                                    //当前弹药量
    private int curPrepareAmmo;                       //当前后备弹药量
    private float shootingIntervalTimer = 0;    //射击间隔计时器
    private float brustTimeTimer = 0;               //brust射击间隔计时器
    private int brustCounter;                             //brust计数器


    [Header("发射物")]
    [SerializeField] public Transform fireSocket;           
    [SerializeField] private GameObject bulletPrefab;   
    public float bulletVelocity = 100;                              
    public float inertialVelocityMultipler = 10;               
    public bool constantSpeed = false;                             


    //本来想做个委托外包出去，想了想不如直接集成在类里得了
    [Header("特效效果")]
    [SerializeField] private GameObject fireFX;
    private CinemachineImpulseSource impulseSource;
    public float cameraShakeMultipler = 1f;


    [Header("音频")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private float soundMultipler = 1f;


    //玩家的输入对应的委托转发
    public UnityAction onFireStart, onFiring, onFireEnd, onReload;
    //换弹时的委托，与UI通信使用
    public UnityAction onReloadStart, onReloadEnd;
    public UnityAction<float> onReloading;
    public UnityAction<int, int> onAmmoChanged;
    //是否选中
    public UnityAction<bool> onSelectChanged;

    //自己的状态机
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

        //初始化数据
        curAmmo = maxAmmo;
        curPrepareAmmo = maxPrepareAmmo;
        brustTimeTimer = brustTime;
        brustCounter = brustNum;

        //状态初始化
        idleState = new WeaponIdleState(stateMachine, this);
        fireState = new WeaponFireState(stateMachine, this);
        reloadState = new WeaponReloadState(stateMachine, this);

        //自身赋值到Controller方便其他组件引用
        PlayerController.Ins.weapons[weaponSelectIndex] = this;
    }

    private void Start()
    {
        //玩家操作本Weapon
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

        //初始化状态机
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

        //发射投射物
        Bullet bullet = Instantiate(bulletPrefab, fireSocket.position, fireSocket.rotation).GetComponent<Bullet>();
        bullet.Init(bulletVelocity, constantSpeed, new Vector3(owner.velocity.x, 0, owner.velocity.z) * inertialVelocityMultipler, owner.flag);

        //数据更新
        ModifyAmmo(-1);
        shootingIntervalTimer = shootingInterval;
        if (fireMode == FireMode.Brust)                 //如果是Brust模式
        {
            brustCounter--;
            if (brustCounter <= 0)
                brustTimeTimer = brustTime;
        }

        //播放枪口特效
        Instantiate(fireFX, fireSocket.position, fireSocket.rotation);

        //震动！
        impulseSource.m_DefaultVelocity.x = Random.Range(-1f, 1f) * cameraShakeMultipler;
        impulseSource.m_DefaultVelocity.y = Random.Range(-1f, 1f) * cameraShakeMultipler;
        impulseSource.m_DefaultVelocity.z = Random.Range(-1f, 1f) * cameraShakeMultipler;
        impulseSource.GenerateImpulse();

        //枪口音效
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

        //没有子弹
        if (!HaveAmmo())
            return false;

        //没有结束冷却
        if (shootingIntervalTimer > 0)
            return false;

        //如果在Brust模式
        //如果Counter小于等于0，说明打完了，否则不管
        //如果打完了并且还没过brust冷却，那就不能打
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
