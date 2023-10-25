using RosMessageTypes.Geometry;
using RosMessageTypes.Telecobot_ros_unity;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class PubSourceDestination : MonoBehaviour
{
    private static readonly string TopicName = "/telecobot_joints";
    private static readonly Quaternion PickOrientation = Quaternion.Euler(90, 90, 0);

    public UrdfJointRevolute[] jointArticulationBodies;
    public GameObject target;
    private ROSConnection m_Ros;
    private TelecobotMoveitJointsMsg jointMsg;

    void Start()
    {
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<TelecobotMoveitJointsMsg>(TopicName);
        jointMsg = new TelecobotMoveitJointsMsg();
    }

    public void Publish()
    {
        for (var i = 0; i < jointArticulationBodies.Length; i++)
        {
            jointMsg.joints[i] = jointArticulationBodies[i].GetPosition();
        }

        jointMsg.goal_pose = new PoseMsg
        {
            position = target.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(90, target.transform.eulerAngles.y, 0).To<FLU>()
        };

        m_Ros.Publish(TopicName, jointMsg);
    }
}