using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponReloadState : WeaponState
{
    private float reloadTimer;

    public WeaponReloadState(StateMachine stateMachine, Weapon weapon) : base(stateMachine, weapon)
    {
    }

    public override void Enter()
    {
        base.Enter();

        reloadTimer = weapon.reloadTime;
        weapon.onReloadStart?.Invoke();
    }

    public override void Update()
    {
        base.Update();

        reloadTimer -= Time.deltaTime;

        weapon.onReloading?.Invoke(reloadTimer);

        if( reloadTimer < 0 )
        {
            weapon.Reload();
            weapon.onReloadEnd?.Invoke();
            stateMachine.ChangeState(weapon.idleState);
        }
    }
}
