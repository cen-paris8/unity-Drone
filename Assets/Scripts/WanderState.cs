using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WanderState : BaseState
{
    private Vector3? _destination;
    private float stopDistance = 1f;
    private float turnSpeed = 1f;
    private readonly LayerMask _layerMask = LayerMask.NameToLayer("Walls");
    private float _rayDistance = 3.5f;
    private Quaternion _desiredRotation;
    private Vector3 _direction;
    private Drone _drone;

    private Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    private Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    public WanderState(Drone drone) : base(drone.gameObject)
    {
        _drone = drone;
    }


    public override Type Tick()
    {
        
        var chaseTarget = CheckForAggro();
        
        if (chaseTarget != null)
        {
            //Debug.Log("chaseTarget != null");
            _drone.SetTarget((Transform)chaseTarget);
            return typeof(ChaseState);
        }
        
        if (_destination.HasValue == false ||
                Vector3.Distance(transform.position, _destination.Value) <= stopDistance)
        {
            //Debug.Log("FindRandomDestination");
            FindRandomDestination();
            //Debug.Log("LayerMask : " + _layerMask);
            //Debug.Log("~LayerMask : " + ~_layerMask);
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, _desiredRotation, Time.deltaTime * turnSpeed);

        if (IsForwardBlocked())
        {
            //Debug.Log("IsForwardBlocked");
            transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, 0.2f);
        }
        
        else
        {
          
            transform.Translate(translation: Vector3.forward * Time.deltaTime * GameSettings.DroneSpeed);

        }
        
        // Debug.DrawRay(start: transform.position, dir: _direction * _rayDistance, Color.red);
        while (IsPathBlocked())
        {
            FindRandomDestination();
            //Debug.Log("WALL!");
        }

        return null;

    }

    private bool IsForwardBlocked()
    {
        // Original
        Ray ray = new Ray(origin: transform.position, direction: transform.forward);
        bool isSomething = Physics.SphereCast(ray, radius: 0.5f, _rayDistance, _layerMask);  //, 
        return isSomething;

    }

    private bool IsPathBlocked()
    {

        Ray ray = new Ray(origin: transform.position, _direction);
        var hitSomething = Physics.RaycastAll(ray, _rayDistance); //RaycastAll

        //if (hitSomething.Any()) //
        //{
        //    Debug.Log("hitSomething : " + hitSomething.ToString());
            
        //}
        return hitSomething.Any(); //.Any();


    }

    private void FindRandomDestination()
    {

        float aggroRadius = 10f;

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;


        Vector3 testPosition = testPosition = (transform.position + (transform.forward * 4f))
                + new Vector3(x: UnityEngine.Random.Range(-4.5f, 4.5f), y: 0f, z: UnityEngine.Random.Range(-4.5f, 4.5f));
        //New
        if (Physics.SphereCast(pos, radius: 10f, direction, out hit, aggroRadius))
        {
            //Debug.Log("Check For Aggro hit : " + hit);
            //Debug.Log("hit.collider" + hit.collider);
            //Debug.Log("hit.distance" + hit.distance);
            var drone = hit.collider.GetComponent<Drone>();
            if (drone != null && drone.Team != gameObject.GetComponent<Drone>().Team)
            {
                testPosition = hit.transform.position;
            }
        }
        //New

            //testPosition = (transform.position + (transform.forward * 4f))
            //   + new Vector3(x: UnityEngine.Random.Range(-4.5f, 4.5f), y: 0f, z: UnityEngine.Random.Range(-4.5f, 4.5f));

            _destination = new Vector3(testPosition.x, y: 1f, testPosition.z);

        _direction = Vector3.Normalize(_destination.Value - transform.position);
        _direction = new Vector3(_direction.x, y: 0f, _direction.z);
        _desiredRotation = Quaternion.LookRotation(_direction);
    }

    

    private Transform CheckForAggro()
    {
        float aggroRadius = 5f;

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (var i = 0; i < 24; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, aggroRadius))
            {
                //Debug.Log("Check For Aggro hit : " + hit);
                //Debug.Log("hit.collider" + hit.collider);
                //Debug.Log("hit.distance" + hit.distance);
                var drone = hit.collider.GetComponent<Drone>();
                if (drone != null && drone.Team != gameObject.GetComponent<Drone>().Team)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    return drone.transform;
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * hit.distance, Color.white);
            }
            direction = stepAngle * direction;
        }
        return null;

    }

}
