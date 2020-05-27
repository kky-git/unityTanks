using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;      //玩家编号
    public float m_Speed = 12f;     //坦克速度
    public float m_TurnSpeed = 180f;        //转向速度
    public AudioSource m_MovementAudio;     //坦克的音效
    public AudioClip m_EngineIdling;        //坦克停止的音效
    public AudioClip m_EngineDriving;       //坦克移动的音效
    public float m_pichRange = 0.2f;        //音调

    private string m_MovementAxisName;      //前后（垂直）控制编号
    private string m_TurnAxisName;      //左右（水平）控制编号
    private Rigidbody m_Rigidbody; 
    private float m_MovementInputValue;     //前后（垂直）输入
    private float m_TurnInputValue;     //左右（水平）输入
    private float m_OriginalPitch;      //原始音高


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }
    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }
    private void EngineAudio()
    {  //播放正确的音效
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_pichRange, m_OriginalPitch + m_pichRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_pichRange, m_OriginalPitch + m_pichRange);
                m_MovementAudio.Play();
            }
        }

    }
    private void FixedUpdate()
    {
        Move();
        Turn();
    }
    private void Move()
    {       //确定坦克的移动位置
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }
    private void Turn()
    {       //确定坦克的旋转角度      
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}
