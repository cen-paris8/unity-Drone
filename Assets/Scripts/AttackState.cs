using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float _attackReadyTimer;
    private Drone _drone;

    public AttackState(Drone drone) : base(drone.gameObject)
    {
        _drone = drone;
    }

    public override Type Tick()
    {
            if (_drone.Target == null)
                return typeof(WanderState);

            _attackReadyTimer -= Time.deltaTime;

            if(_attackReadyTimer <= 0f)
            {
                //Debug.Log("Attack!");
                _drone.FireWeapon();
            }
            return null;
    }
    
}
