using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class WeaponState : State
{
    protected Weapon weapon;

    public WeaponState(StateMachine stateMachine, Weapon weapon) : base(stateMachine)
    {
        this.weapon = weapon;
    }


}
