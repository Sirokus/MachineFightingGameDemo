using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    //Third Person Camera
    [SerializeField] private CinemachineVirtualCamera thirdPersonCamera;
    private Cinemachine3rdPersonFollow thirdCameraBody;
    public float freeLookSide = 0;
    public float freeLookDistance = 20;
    float cameraSide;
    float cameraDistance;


    //First Peroson Camera
    [SerializeField] private CinemachineVirtualCamera firstPersonCamera;
    private Cinemachine3rdPersonFollow firstCameraBody;


    private Quaternion lastQuaternion;

    private void Awake()
    {
        thirdCameraBody = thirdPersonCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        cameraSide = thirdCameraBody.CameraSide;
        cameraDistance = thirdCameraBody.CameraDistance;


        firstCameraBody = firstPersonCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleFreeLook();
        HandleSwitchView();
    }

    private void HandleFreeLook()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //����������
            lastQuaternion = PlayerController.GetControllerRotation();
            PlayerController.SetPause(true);

            //�����˳Ʋ���
            thirdCameraBody.CameraSide = freeLookSide;
            thirdCameraBody.CameraDistance = freeLookDistance;

            //����׼�ǣ����Ӿ�Bug��
            foreach(var hair in UIManager.Ins.crossHairs)
            {
                hair.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            //�������ָ�
            PlayerController.SetControllerRotation(lastQuaternion);
            PlayerController.SetPause(false);

            //�����˳Ʋ���
            thirdCameraBody.CameraSide = cameraSide;
            thirdCameraBody.CameraDistance = cameraDistance;

            //��ʾUI
            foreach (var hair in UIManager.Ins.crossHairs)
            {
                hair.gameObject.SetActive(PlayerController.Ins.weapons[hair.weaponIndex].selected);
            }
        }
    }

    private void HandleSwitchView()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            firstPersonCamera.gameObject.SetActive(!firstPersonCamera.gameObject.activeSelf);
            thirdPersonCamera.gameObject.SetActive(!thirdPersonCamera.gameObject.activeSelf);
        }
    }
}
