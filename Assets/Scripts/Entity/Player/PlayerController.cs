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

    //����緽���ȡ������
    [Header("����")]
    public Player player;
    public Dictionary<int, Weapon> weapons = new Dictionary<int, Weapon>();

    //�涯����
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

        //��׽����
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

        //����������й������
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
            //��������ת
            RotX -= mouseY;
            RotY += mouseX;

            //���ƿ������ĸ�����
            RotX = RotX > 180 ? RotX - 360 : RotX;
            RotX = Mathf.Clamp(RotX, -89, 89);

            transform.rotation = Quaternion.Euler(RotX, RotY, 0f);

            //������Ҫ���ٿ�������ת�����壬���������ת��ֵ
            foreach (var obj in syncControllerRotationObjects)
            {
                if (paused && !obj.ignorePause)
                    continue;

                //������ԭʼ�Ƕ�	0 ~ 360
                float x = transform.rotation.eulerAngles.x;
                float y = transform.rotation.eulerAngles.y;

                //��ת������丸���ĽǶȣ���Ϊdir���ѣ�
                Vector3 dir = obj.syncTransform.eulerAngles;
                Vector3 parentDir = obj.parentTransform ? obj.parentTransform.eulerAngles : obj.syncTransform.parent.eulerAngles;

                //�������Ƕȣ�ӳ�䵽��-180 ~ 180��
                float conDirX = x > 180 ? x - 360 : x;
                float conDirY = y > 180 ? y - 360 : y;

                //��ǰ����ת�Ƕȣ�ӳ�䵽��-180 ~ 180��
                float dirX = dir.x > 180 ? dir.x - 360 : dir.x;
                float dirY = dir.y > 180 ? dir.y - 360 : dir.y;

                //��������ת�Ƕȣ�ӳ�䵽��-180 ~ 180��
                float parentDirX = parentDir.x > 180 ? parentDir.x - 360 : parentDir.x;
                float parentDirY = parentDir.y > 180 ? parentDir.y - 360 : parentDir.y;

                //��ǰ�͸�������ԽǶȣ����ڼ������ƽǶȣ�
                float angleX = parentDirX - dirX;
                float angleY = dirY - parentDirY;

                //��������ƽ����ת
                //����������ת�Ƕ�(ƫ��һ�£���dirΪ��㣬������㵽Ŀ������Ƕ�)
                float targetAngleX = (x - dir.x + 360) % 360;
                float targetAngleY = (y - dir.y + 360) % 360;
                //���Ž�0~360ӳ��Ϊ -180 ~ 180����Ϊ���Ե�ǰ����Ϊԭ���£���������ԭ�����ԽǶȲ�
                targetAngleX = targetAngleX > 180 ? targetAngleX - 360 : targetAngleX;
                targetAngleY = targetAngleY > 180 ? targetAngleY - 360 : targetAngleY;

                if (obj.SyncX)
                {
                    Vector2 rotation = obj.syncTransform.rotation.eulerAngles;

                    //�����ǰ�ǶȲ����ֵС����ת���ʣ�ֱ��ת��Ŀ��Ƕ�
                    if (Mathf.Abs(targetAngleX) <= obj.RateY * Time.deltaTime)
                    {
                        rotation.x = x;
                        angleX = parentDirX - conDirX;
                    }
                    //Ŀ���ڵ�ǰ���峯���ұߣ���������Ƕ�
                    else if (targetAngleX > 0)
                    {
                        rotation.x += obj.RateX * Time.deltaTime;
                        angleX -= obj.RateX * Time.deltaTime;
                    }
                    //Ŀ���ڵ�ǰ���峯����ߣ���С����Ƕ�
                    else
                    {
                        rotation.x -= obj.RateX * Time.deltaTime;
                        angleX += obj.RateX * Time.deltaTime;
                    }

                    //�Ƕ������жϣ�angleX�Ѿ��Ǳ任��ĽǶ���
                    if ((obj.SyncX && angleX >= obj.LimitXRange.min && angleX <= obj.LimitXRange.max) || !obj.LimitX)
                    {
                        obj.syncTransform.rotation = Quaternion.Euler(rotation);
                    }
                }

                //����������ļ���ͬ��
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
