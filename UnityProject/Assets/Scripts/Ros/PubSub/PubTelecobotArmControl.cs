using RosMessageTypes.Geometry;
using RosMessageTypes.Telecobot_ros_unity;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class PubTelecobotArmControl : MonoBehaviour
{
    private static readonly string TopicName = "/telecobot_arm_control";
    private static readonly Quaternion PickOrientation = Quaternion.Euler(0, 0, 0);

    public GameObject endEffector;
    public GameObject armBaseLink;
    [HideInInspector]
    public ArmControlMode armControlMode;
    [HideInInspector]
    public Transform target = null;

    [SerializeField]
    double m_PublishRateHz = 1f;

    double m_LastPublishTimeSeconds;

    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;

    bool ShouldPublishMessage => RosClock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    private ROSConnection m_Ros;
    private TelecobotArmControlMsg armMsg;
    private bool isConnected=false;
    bool flag;

    void Start()
    {
        isConnected = false;
        flag = false;
        armControlMode = ArmControlMode.Home;
    }

    public void OnRosConnect()
    {
        m_Ros = ROSConnection.GetOrCreateInstance();
        if (flag == false)
        {
            m_Ros.RegisterPublisher<TelecobotArmControlMsg>(TopicName);
            flag = true;
        }
        armMsg = new TelecobotArmControlMsg();
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
        armMsg.arm_control_mode = (int)armControlMode;

        m_Ros.Publish(TopicName, armMsg);
        m_LastPublishTimeSeconds = RosClock.FrameStartTimeInSeconds;
    }

    public void PublishTransform()
    {
        armMsg.arm_control_mode = (int)ArmControlMode.PublishTarget;
        // 目標位置を格納
        if (target != null)
        {
            armMsg.goal_pose = new PoseMsg
            {
                //position = target.transform.position.To<FLU>(),
                position = target.localPosition.To<FLU>(),
                //orientation = Quaternion.Euler(target.transform.eulerAngles.x,
                //                               target.transform.eulerAngles.y,
                //                               target.transform.eulerAngles.z).To<FLU>()
                orientation = Quaternion.Euler(90, target.eulerAngles.y, 0).To<FLU>()
            };
        };
        // エンドエフェクタの位置を格納
        armMsg.end_effector_pose = new PoseMsg
        {
            position = endEffector.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(endEffector.transform.eulerAngles.x,
                                            endEffector.transform.eulerAngles.y,
                                            endEffector.transform.eulerAngles.z).To<FLU>()
        };
        m_Ros.Publish(TopicName, armMsg);
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