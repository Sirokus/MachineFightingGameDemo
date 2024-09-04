using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WeaponFireState : WeaponCommonState
{
    public bool readyEnd;

    public WeaponFireState(StateMachine stateMachine, Weapon weapon) : base(stateMachine, weapon)
    {
    }

    public override void Enter()
    {
        base.Enter();

        weapon.onFireEnd += OnFireEnd;

        FireOrReload();

        readyEnd = false;
    }

    public override void Exit()
    {
        base.Exit();

        weapon.onFireEnd -= OnFireEnd;
    }

    private void OnFireEnd()
    {
        if (weapon.fireMode != Weapon.FireMode.Brust)
            stateMachine.ChangeState(weapon.idleState);
        readyEnd = true;
    }

    public override void Update()
    {
        base.Update();

        //爆发模式
        if (weapon.fireMode == Weapon.FireMode.Brust)
        {
            if (weapon.CanBrust())
                FireOrReload();
            else if(readyEnd)
                stateMachine.ChangeState(weapon.idleState);
        }
        else if (weapon.fireMode == Weapon.FireMode.Auto)
            FireOrReload();
    }

    private void FireOrReload()
    {
        if (weapon.HaveAmmo())
        {
            weapon.Fire();
            if (!weapon.HaveAmmo())
                stateMachine.ChangeState(weapon.reloadState);
        }
        else
            stateMachine.ChangeState(weapon.reloadState);
    }
}
