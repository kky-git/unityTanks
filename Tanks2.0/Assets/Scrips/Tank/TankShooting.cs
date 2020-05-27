using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;      //玩家编号
    public Rigidbody m_shell;       
    public Transform m_FireTransform;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;        //最小射击力度
    public float m_MaxLAunchForce = 30f;        //最大射击力度
    public float m_MaxChargeTime = 0.75f;       //射击最长按压时间

    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        m_ChargeSpeed = (m_MaxLAunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    // Update is called once per frame
    void Update()
    {
        m_AimSlider.value = m_MinLaunchForce;
        if (m_CurrentLaunchForce >= m_MaxLAunchForce && !m_Fired)
        {       //按压产生的力度大于等于最大力度，且子弹未发射
            m_CurrentLaunchForce = m_MaxLAunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))
        {       //当发射按键按下时，开始设置发射力度为最小值，按压音效启动
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {       //发射按键处于按压状态时，设置发射力度随着时间增加
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {       //发射按键抬起时，发射子弹
            Fire();
        }
    }
    private void Fire()
    {
        m_Fired = true;
        Rigidbody shellInstance = Instantiate(m_shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}
