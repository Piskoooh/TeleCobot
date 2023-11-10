using RosMessageTypes.Geometry;
using RosMessageTypes.TelecobotRosUnity;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class PubTelecobotBaseControl : MonoBehaviour
{
    private static readonly string TopicName = "/telecobot_base_control";
    private static readonly Quaternion PickOrientation = Quaternion.Euler(0, 0, 0);

    public UrdfJointRevolute[] jointArticulationBodies;
    public GameObject endEffector;
    public GameObject BaseLink;
    [HideInInspector]
    public BaseControlMode baseControlMode;
    [HideInInspector]
    public Transform target = null;

    [SerializeField]
    double m_PublishRateHz = 1f;

    double m_LastPublishTimeSeconds;

    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;

    bool ShouldPublishMessage => RosClock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    private ROSConnection m_Ros;
    private TelecobotBaseControlMsg baseMsg;
    private bool isConnected=false;
    bool flag;

    void Start()
    {
        isConnected = false;
        flag = false;
        baseControlMode = BaseControlMode.Sleep;
    }

    public void OnRosConnect()
    {
        m_Ros = ROSConnection.GetOrCreateInstance();
        if (flag == false)
        {
            m_Ros.RegisterPublisher<TelecobotBaseControlMsg>(TopicName);
            flag = true;
        }
        baseMsg = new TelecobotBaseControlMsg();
        isConnected = true;

        m_LastPublishTimeSeconds = RosClock.time + PublishPeriodSeconds;
    }

    public void OnRosDisconnected()
    {
        isConnected = false;
    }

    public void Publish()
    {
        // Unity上で選択されているモードをMsgに入力
        baseMsg.base_control_mode = (int)baseControlMode;

        m_Ros.Publish(TopicName, baseMsg);
        m_LastPublishTimeSeconds = RosClock.FrameStartTimeInSeconds;
    }

    public void PublishTransform()
    {
        baseMsg.base_control_mode = (int)ArmControlMode.PublishTarget;

        // 目標位置を格納
        if (target != null)
        {
            baseMsg.goal = new PoseMsg
            {
                //position = target.transform.position.To<FLU>(),
                position = target.position.To<FLU>(),
                orientation = Quaternion.Euler(target.transform.eulerAngles.x,
                                               target.transform.eulerAngles.y,
                                               target.transform.eulerAngles.z).To<FLU>()
            };
        };
        m_Ros.Publish(TopicName, baseMsg);
    }

    //常にパブリッシュし続ける
    private void Update()
    {
        if (isConnected)
        {
            if (ShouldPublishMessage)
            {
                Publish();
            }
        }
    }
}