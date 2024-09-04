using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class Player : Entity
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        PlayerController controller = PlayerController.Ins;
        controller.player = this;

        //°ó¶¨¶¯×÷
        controller.jump.performed += (context) => onJump?.Invoke();
        controller.fire.started += (context) =>
        {
            onFireStart.Invoke();
            firing = true;
        };
        controller.fire.canceled += (context) =>
        {
            onFireEnd?.Invoke();
            firing = false;
        };
        controller.reload.performed += (context) => onReload?.Invoke();

        controller.allSelect.performed += (context) => onAllSelect?.Invoke();
        controller.selectFirst.performed += (context) => onSelect?.Invoke(0);
        controller.selectSecond.performed += (context) => onSelect?.Invoke(1);
        controller.selectThird.performed += (context) => onSelect?.Invoke(2);
    }

    protected override void Update()
    {
        base.Update();

        Vector2 input = PlayerController.Ins.move.ReadValue<Vector2>();
        onMove?.Invoke(input);
    }
}
