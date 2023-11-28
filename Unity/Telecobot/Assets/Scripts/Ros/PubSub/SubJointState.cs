using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class SubJointState : MonoBehaviour
{
    public ArticulationBody left_wheel_joint;
    public ArticulationBody right_wheel_joint;
    public ArticulationBody waist;
    public ArticulationBody shoulder;
    public ArticulationBody elbow;
    public ArticulationBody wrist_angle;
    public ArticulationBody wrist_rotate;
    public ArticulationBody gripper;
    public ArticulationBody left_finger;
    public ArticulationBody right_finger;
    public ArticulationBody pan;
    public ArticulationBody tilt;

    private ArticulationBody[] articulationBodies;
    public string topicName = "/locobot/joint_states";
    private ROSConnection ros;

    public void OnRosConnect()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<JointStateMsg>(topicName, GetJointPositions);

        articulationBodies = new ArticulationBody[12];
        articulationBodies[0] = left_wheel_joint;
        articulationBodies[1] = right_wheel_joint;
        articulationBodies[2] = waist;
        articulationBodies[3] = shoulder;
        articulationBodies[4] = elbow;
        articulationBodies[5] = wrist_angle;
        articulationBodies[6] = wrist_rotate;
        articulationBodies[7] = gripper;
        articulationBodies[8] = left_finger;
        articulationBodies[9] = right_finger;
        articulationBodies[10] = pan;
        articulationBodies[11] = tilt;

    }

    private void GetJointPositions(JointStateMsg sensorMsg)
    {
        StartCoroutine(SetJointValues(sensorMsg));
    }
    IEnumerator SetJointValues(JointStateMsg message)
    {
        for (int i = 0; i < message.name.Length; i++)
        {
            var joint1XDrive = articulationBodies[i].xDrive;
            if (i > 1) //skip wheel joints.
            {
                if (i == 8 || i == 9)//finger joints
                {
                    joint1XDrive.target = (float)(message.position[i]);
                }
                else
                {
                    joint1XDrive.target = (float)(message.position[i]) * Mathf.Rad2Deg;
                }
                articulationBodies[i].xDrive = joint1XDrive;
                Debug.Log("message.name:" +message.name[i] + "  joint1XDrive.target:"+ joint1XDrive.target);
            }
        }

        yield return new WaitForSeconds(0.0f);
    }

    public void UnSub()
    {
        ros.Unsubscribe(topicName);
    }
}