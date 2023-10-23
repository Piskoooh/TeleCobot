using System.Collections;
using RosMessageTypes.Moveit;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class SubMoveitResponce : MonoBehaviour
{
    private static readonly string TopicName = "/moveit_response";
    private static readonly Quaternion PickOrientation = Quaternion.Euler(90, 90, 0);

    public ArticulationBody[] jointArticulationBodies;
    private ROSConnection m_Ros;

    public void OnRosConnect()
    {
        m_Ros = ROSConnection.GetOrCreateInstance();

        m_Ros.Subscribe<RobotTrajectoryMsg>(TopicName, Callback);
    }

    void Callback(RobotTrajectoryMsg trajectory)
    {
        if (trajectory != null && trajectory.joint_trajectory.points.Length > 0)
        {
            print("success>>>");
            StartCoroutine(ExecuteTrajectories(trajectory));
        }
        else
        {
            print("failed>>>");
        }
    }

    IEnumerator ExecuteTrajectories(RobotTrajectoryMsg trajectory)
    {
        foreach (var t in trajectory.joint_trajectory.points)
        {
            float[] result = new float[5];
            for (var i = 0; i < t.positions.Length; i++)
            {
                result[i] = (float)t.positions[i] * Mathf.Rad2Deg;
            }
            for (var i = 0; i < jointArticulationBodies.Length; i++)
            {
                var joint1XDrive = jointArticulationBodies[i].xDrive;
                joint1XDrive.target = result[i];
                jointArticulationBodies[i].xDrive = joint1XDrive;
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);
    }

    public void UnSub()
    {
        m_Ros.Unsubscribe(TopicName);
    }
}