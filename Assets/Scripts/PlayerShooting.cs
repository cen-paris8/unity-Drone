using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    //public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 0f;
    public float m_MaxLaunchForce = 10f;
    public float m_MaxChargeTime = 0.75f;


    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;
    private Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    private Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    private Drone _drone;


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
        
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        _drone = GetComponent<Drone>();
    }


    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // at max charge, not fired
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            // have we pressed fire for the first time ?
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            // Holding the fire button, not yet fired
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // we released the button, having not fired yet
            Fire();
        }
    }

    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;

        float aggroRadius = 5f;

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var pos = transform.position;
        //var direction = pos + new Vector3(1, 0, 1);
        List<Vector3> directions = new List<Vector3>()
        {
            new Vector3(1,0,0),
            new Vector3(1,0,1),
            new Vector3(0,0,1)
        };
        foreach(Vector3 direction in directions)
        {        
            if (Physics.Raycast(pos, direction, out hit, aggroRadius))
            {
                //Debug.Log("Check For Aggro hit : " + hit);
                //Debug.Log("hit.collider" + hit.collider);
                //Debug.Log("hit.distance" + hit.distance);
                var drone = hit.collider.GetComponent<Drone>();
                if (drone != null && drone.Team != gameObject.GetComponent<Drone>().Team)
                {
                    _drone.SetTarget(drone.transform);

                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    
                }
            }
        }
        if (_drone.Target != null)
        {
            _drone.FireWeapon();
        }
        
    }

}