using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WeaponCommonState : WeaponState
{
    public WeaponCommonState(StateMachine stateMachine, Weapon weapon) : base(stateMachine, weapon)
    {
    }

    public override void Enter()
    {
        base.Enter();

        weapon.onReload += OnReload;
    }

    public override void Exit()
    {
        base.Exit();

        weapon.onReload -= OnReload;
    }

    private void OnReload()
    {
        if(weapon.CanReload())
            stateMachine.ChangeState(weapon.reloadState);
    }
}
