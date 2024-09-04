using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static PlayerController m_instance;
    public static PlayerController Ins { get => m_instance; }

    //供外界方便获取的引用
    [Header("引用")]
    public Player player;
    public Dictionary<int, Weapon> weapons = new Dictionary<int, Weapon>();

    //随动物体
    [System.Serializable]
    public class Limit
    {
        public float min = -90;
        public float max = 90;
    }
    [System.Serializable]
    public class SyncObject
    {
        public Transform syncTransform;
        public Transform parentTransform;
        public bool ignorePause = false;


        [Header("Horizontal")]
        public bool SyncY = true;
        public float RateY = 3600;
        public bool LimitY = false;
        public Limit LimitYRange;


        [Header("Vertical")]
        public bool SyncX = true;
        public float RateX = 3600;
        public bool LimitX = false;
        public Limit LimitXRange;
    }
    protected float RotX = 0, RotY = 0;
    protected bool paused = false;
    public List<SyncObject> syncControllerRotationObjects;

    //input
    public InputAction move;
    public InputAction jump;
    public InputAction fire;
    public InputAction reload;
    public InputAction allSelect, selectFirst, selectSecond, selectThird;

    protected virtual void Awake()
    {
        if(m_instance)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    protected virtual void Start()
    {
        move = new InputAction("PlayerMovement");
        move.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");

        jump = new InputAction("PlayerJump");
        jump.AddBinding("<Keyboard>/space");

        fire = new InputAction("PlayerFire");
        fire.AddBinding("<Mouse>/leftButton");

        reload = new InputAction("PlayerReload");
        reload.AddBinding("<Keyboard>/R");

        allSelect = new InputAction("AllSelect");
        allSelect.AddBinding("<Keyboard>/BackQuote");

        selectFirst = new InputAction("SelectFirst");
        selectFirst.AddBinding("<Keyboard>/1");

        selectSecond = new InputAction("SelectSecond");
        selectSecond.AddBinding("<Keyboard>/2");

        selectThird = new InputAction("SelectThird");
        selectThird.AddBinding("<Keyboard>/3");

        move.Enable();
        jump.Enable();
        fire.Enable();
        reload.Enable();
        allSelect.Enable();
        selectFirst.Enable();
        selectSecond.Enable();
        selectThird.Enable();
    }

    protected virtual void Update()
    {
        bool unlockPressed = false, lockPressed = false;

        float mouseX = 0, mouseY = 0;

        //捕捉输入
        if (Mouse.current != null)
        {
            var delta = Mouse.current.delta.ReadValue() / 15.0f;
            mouseX += delta.x;
            mouseY += delta.y;
            lockPressed = Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame;
        }
        if (Keyboard.current!= null)
        {
            unlockPressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        }

        //按照输入进行光标设置
        if (unlockPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (lockPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            //控制器旋转
            RotX -= mouseY;
            RotY += mouseX;

            //限制控制器的俯仰角
            RotX = RotX > 180 ? RotX - 360 : RotX;
            RotX = Mathf.Clamp(RotX, -89, 89);

            transform.rotation = Quaternion.Euler(RotX, RotY, 0f);

            //遍历需要跟踪控制器旋转的物体，对其进行旋转赋值
            foreach (var obj in syncControllerRotationObjects)
            {
                if (paused && !obj.ignorePause)
                    continue;

                //控制器原始角度	0 ~ 360
                float x = transform.rotation.eulerAngles.x;
                float y = transform.rotation.eulerAngles.y;

                //旋转物体和其父级的角度（名为dir而已）
                Vector3 dir = obj.syncTransform.eulerAngles;
                Vector3 parentDir = obj.parentTransform ? obj.parentTransform.eulerAngles : obj.syncTransform.parent.eulerAngles;

                //控制器角度（映射到了-180 ~ 180）
                float conDirX = x > 180 ? x - 360 : x;
                float conDirY = y > 180 ? y - 360 : y;

                //当前的旋转角度（映射到了-180 ~ 180）
                float dirX = dir.x > 180 ? dir.x - 360 : dir.x;
                float dirY = dir.y > 180 ? dir.y - 360 : dir.y;

                //父级的旋转角度（映射到了-180 ~ 180）
                float parentDirX = parentDir.x > 180 ? parentDir.x - 360 : parentDir.x;
                float parentDirY = parentDir.y > 180 ? parentDir.y - 360 : parentDir.y;

                //当前和父级的相对角度（用于计算限制角度）
                float angleX = parentDirX - dirX;
                float angleY = dirY - parentDirY;

                //用于线性平滑旋转
                //控制器的旋转角度(偏移一下，以dir为起点，方便计算到目标所需角度)
                float targetAngleX = (x - dir.x + 360) % 360;
                float targetAngleY = (y - dir.y + 360) % 360;
                //接着将0~360映射为 -180 ~ 180，此为在以当前朝向为原点下，控制器与原点的相对角度差
                targetAngleX = targetAngleX > 180 ? targetAngleX - 360 : targetAngleX;
                targetAngleY = targetAngleY > 180 ? targetAngleY - 360 : targetAngleY;

                if (obj.SyncX)
                {
                    Vector2 rotation = obj.syncTransform.rotation.eulerAngles;

                    //如果当前角度差绝对值小于旋转速率，直接转到目标角度
                    if (Mathf.Abs(targetAngleX) <= obj.RateY * Time.deltaTime)
                    {
                        rotation.x = x;
                        angleX = parentDirX - conDirX;
                    }
                    //目标在当前物体朝向右边，增大自身角度
                    else if (targetAngleX > 0)
                    {
                        rotation.x += obj.RateX * Time.deltaTime;
                        angleX -= obj.RateX * Time.deltaTime;
                    }
                    //目标在当前物体朝向左边，减小自身角度
                    else
                    {
                        rotation.x -= obj.RateX * Time.deltaTime;
                        angleX += obj.RateX * Time.deltaTime;
                    }

                    //角度限制判断，angleX已经是变换后的角度了
                    if ((obj.SyncX && angleX >= obj.LimitXRange.min && angleX <= obj.LimitXRange.max) || !obj.LimitX)
                    {
                        obj.syncTransform.rotation = Quaternion.Euler(rotation);
                    }
                }

                //该轴与上面的计算同理
                if (obj.SyncY)
                {
                    Vector2 rotation = obj.syncTransform.rotation.eulerAngles;

                    if (Mathf.Abs(targetAngleY) <= obj.RateY * Time.deltaTime)
                    {
                        rotation.y = y;
                        angleY = parentDirY - conDirY;
                    }
                    else if (targetAngleY > 0)
                    {
                        rotation.y += obj.RateY * Time.deltaTime;
                        angleY -= obj.RateY * Time.deltaTime;
                    }
                    else
                    {
                        rotation.y -= obj.RateY * Time.deltaTime;
                        angleY += obj.RateY * Time.deltaTime;
                    }

                    if (!obj.LimitY || (angleY >= obj.LimitYRange.min && angleY <= obj.LimitYRange.max))
                    {
                        obj.syncTransform.rotation = Quaternion.Euler(rotation);
                    }
                }
            }
        }
    }

    public static void SetControllerRotation(float rotX, float rotY)
    {
        m_instance.RotX = rotX;
        m_instance.RotY = rotY;
        m_instance.transform.rotation = Quaternion.Euler(rotX, rotY, 0f);
    }
    public static void SetControllerRotation(Quaternion quaternion)
    {
        SetControllerRotation(quaternion.eulerAngles.x, quaternion.eulerAngles.y);
    }

    public static Quaternion GetControllerRotation() => m_instance.transform.rotation;
    public static Transform GetControllerTransform() => m_instance.transform;
    public static void SetPause(bool paused) => m_instance.paused = paused;
}
