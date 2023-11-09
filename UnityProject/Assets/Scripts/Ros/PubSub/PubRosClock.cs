using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Rosgraph;

//時間を管理するスクリプト。
//Pubするスピード(Hz)を設定している”Scripts/ROS”配下のPubで始まるスクリプトがこのスクリプトを参照する。
//また、ROS側でuse_time_sim=trueとするとこのスクリプトで発行される時間を参照するようになる。
public class PubRosClock : MonoBehaviour
{
    [SerializeField]
    RosClock.ClockMode m_ClockMode;

    [SerializeField, HideInInspector]
    RosClock.ClockMode m_LastSetClockMode;

    [SerializeField]
    double m_PublishRateHz = 100f;

    double m_LastPublishTimeSeconds;

    ROSConnection m_ROS;

    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;

    bool ShouldPublishMessage => RosClock.FrameStartTimeInSeconds - PublishPeriodSeconds > m_LastPublishTimeSeconds;

    private bool isConnected = false;
    bool flag;

    void Start()
    {
        isConnected = false;
        flag = false;
    }

    void OnValidate()
    {
        var clocks = FindObjectsOfType<PubRosClock>();
        if (clocks.Length > 1)
        {
            Debug.LogWarning("Found too many clock publishers in the scene, there should only be one!");
        }

        if (Application.isPlaying && m_LastSetClockMode != m_ClockMode)
        {
            Debug.LogWarning("Can't change ClockMode during simulation! Setting it back...");
            m_ClockMode = m_LastSetClockMode;
        }

        SetClockMode(m_ClockMode);
    }

    void SetClockMode(RosClock.ClockMode mode)
    {
        RosClock.Mode = mode;
        m_LastSetClockMode = mode;
    }

    // Start is called before the first frame update
    public void OnRosConnect()
    {
        SetClockMode(m_ClockMode);
        m_ROS = ROSConnection.GetOrCreateInstance();
        if (flag == false)
        {
            m_ROS.RegisterPublisher<ClockMsg>("/clock");
            flag = true;
        }
    }

    public void OnRosDisconnected()
    {
        isConnected = false;
    }

    void PublishMessage()
    {
        var publishTime = RosClock.time;
        var clockMsg = new TimeMsg
        {
            sec = (uint)publishTime,
            nanosec = (uint)((publishTime - Math.Floor(publishTime)) * RosClock.k_NanoSecondsInSeconds)
        };
        m_LastPublishTimeSeconds = publishTime;
        m_ROS.Publish("/clock", clockMsg);
    }

    //常にパブリッシュし続ける
    private void Update()
    {
        if (isConnected)
        {
            if (ShouldPublishMessage)
            {
                PublishMessage();
            }
        }
    }
}