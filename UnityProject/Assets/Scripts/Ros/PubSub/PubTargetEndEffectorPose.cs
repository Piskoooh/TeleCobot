using RosMessageTypes.Geometry;
using RosMessageTypes.TelecobotRosUnity;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class PubTargetEndEffector : MonoBehaviour
{
    private static readonly string TopicName = "/telecobot_tracking";
    private static readonly Quaternion PickOrientation = Quaternion.Euler(0, 0, 0);

    public UrdfJointRevolute[] jointArticulationBodies;
    public GameObject target;
    public GameObject endEffector;

    [SerializeField]
    double m_PublishRateHz = 1f;

    double m_LastPublishTimeSeconds;

    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;

    bool ShouldPublishMessage => RosClock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    private ROSConnection m_Ros;
    private TelecobotTrackingTargetMsg jointMsg;
    private bool isConnected=false;
    bool flag;

    private void Start()
    {
        isConnected = false;
        flag = false;
    }


    public void OnRosConnect()
    {
        m_Ros = ROSConnection.GetOrCreateInstance();
        if (flag == false)
        {
            m_Ros.RegisterPublisher<TelecobotTrackingTargetMsg>(TopicName);
            flag = true;
        }
        jointMsg = new TelecobotTrackingTargetMsg();
        isConnected = true;

        m_LastPublishTimeSeconds = RosClock.time + PublishPeriodSeconds;
    }

    public void OnRosDisconnected()
    {
        isConnected = false;
    }

    public void Publish()
    {
        // 現在の間接角度を格納
        for (var i = 0; i < jointArticulationBodies.Length; i++)
        {
            jointMsg.joints[i] = jointArticulationBodies[i].GetPosition();
        }
        // 目標位置を格納
        jointMsg.goal_pose = new PoseMsg
        {
            //position = target.transform.position.To<FLU>(),
            position=target.transform.localPosition.To<FLU>(),
            //orientation = Quaternion.Euler(target.transform.eulerAngles.x,
            //                               target.transform.eulerAngles.y,
            //                               target.transform.eulerAngles.z).To<FLU>()
            orientation = Quaternion.Euler(90, target.transform.eulerAngles.y, 0).To<FLU>()

        };
        // エンドエフェクタの位置を格納
        jointMsg.end_effector_pose = new PoseMsg
        {
            position = endEffector.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(endEffector.transform.eulerAngles.x,
                                           endEffector.transform.eulerAngles.y,
                                           endEffector.transform.eulerAngles.z).To<FLU>()
        };

        m_Ros.Publish(TopicName, jointMsg);
        m_LastPublishTimeSeconds = RosClock.FrameStartTimeInSeconds;
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