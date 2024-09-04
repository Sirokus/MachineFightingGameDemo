using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    float RotX = 0f;

    public Transform playerBody;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool unlockPressed = false, lockPressed = false;

        float mouseX = 0,mouseY = 0;

        //捕捉输入
        if(Mouse.current != null )
        {
            var delta = Mouse.current.delta.ReadValue() / 15.0f;
            mouseX += delta.x;
            mouseY += delta.y;
            lockPressed = Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame;
        }
        if(Keyboard.current!= null )
        {
            unlockPressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        }


        //按照输入进行光标设置
        if(unlockPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if(lockPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //进行摄像机输入设置
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            RotX -= mouseY;
            RotX = Mathf.Clamp(RotX, -90f, 90f);

            transform.localRotation = Quaternion.Euler(RotX,0f,0f);

            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
