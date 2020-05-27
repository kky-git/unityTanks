using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;     //相机移动的缓冲时间
    public float m_ScreenEdgeBuffer = 4f;       //屏幕边缘留出的空间，确保坦克不在相机视线的边缘
    public float m_MinSize = 6.5f;      //相机size的最小值确保相机不会无限缩小
    [HideInInspector] public Transform[] m_Targets;       //相机的观察对象数组（坦克）

    private Camera m_Camera;
    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;
    // Start is called before the first frame update
    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {       //移动相机到指定位置
        FindAveragePosition();
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }
    private void FindAveragePosition()
    {       //计算得到当前处于活动状态的坦克的平均位置
        Vector3 averagePos = new Vector3();
        int numTargets = 0;     //观察对象（处于活动状态的）的数量，默认为0
        for(int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;
            averagePos += m_Targets[i].position;
            numTargets++;
        }
        if (numTargets > 0)
            averagePos /= numTargets;
        averagePos.y=transform.position.y;
        m_DesiredPosition = averagePos;
    }
    private void Zoom()
    {
        float requierdSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requierdSize, ref m_ZoomSpeed, m_DampTime);
    } 
    private float FindRequiredSize()
    {       //计算相机的size
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);       //相机应该移动的位置相对与现在位置的坐标
        float size = 0f;
        for(int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);        //坦克相对与现在位置的坐标
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;      //相机应该移动的位置和坦克之间的距离矢量
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);

        }
        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);
        return size;
    }
    public void SetStartPositionAndSize()
    {       //当重置场景中的坦克时，相机自动移动到相应位置设置正确大小
        FindAveragePosition();
        transform.position = m_DesiredPosition;
        m_Camera.orthographicSize = FindRequiredSize();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
