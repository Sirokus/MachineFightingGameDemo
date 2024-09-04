using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WeaponIdleState : WeaponCommonState
{
    public WeaponIdleState(StateMachine stateMachine, Weapon weapon) : base(stateMachine, weapon)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //weapon.onFireStart += OnFireStart;
        weapon.onFiring += OnFireStart;
    }

    public override void Exit()
    {
        base.Exit();

        //weapon.onFireStart -= OnFireStart;
        weapon.onFiring -= OnFireStart;
    }

    private void OnFireStart()
    {
        if (!weapon.selected)
            return;

        if (weapon.HaveAmmo())
            stateMachine.ChangeState(weapon.fireState);
        else if(weapon.HavePrepareAmmo())
            stateMachine.ChangeState(weapon.reloadState);
    }
}
