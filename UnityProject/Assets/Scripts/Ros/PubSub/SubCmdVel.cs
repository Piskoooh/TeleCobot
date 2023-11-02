using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Unity.Robotics.UrdfImporter.Control;


public class SubCmdVel : MonoBehaviour
{
    public ArticulationBody left_wheel_joint;
    public ArticulationBody right_wheel_joint;

    private ArticulationBody[] articulationBodies;
    public string topicName = "/mobile_base/cmd_vel";
    private ROSConnection ros;

    public float maxLinearSpeed = 2; //  m/s
    public float maxRotationalSpeed = 1;//
    public float wheelRadius = 0.03575f; //meters
    public float trackWidth = 0.233f; // meters Distance between tyres
    public float dampingBase = 10;

    public float ROSTimeout = 0.5f;
    private float lastCmdReceived = 0f;

    private RotationDirection direction;
    private float rosLinear = 0f;
    private float rosAngular = 0f;

    private bool isConnected = false;

    private void Start()
    {
        isConnected = false;
    }

    public void OnRosConnect()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<TwistMsg>(topicName, GetTwist);

        isConnected = true;
        articulationBodies = new ArticulationBody[2];
        articulationBodies[0] = left_wheel_joint;
        articulationBodies[1] = right_wheel_joint;

        SetParameters(articulationBodies[0]);
        SetParameters(articulationBodies[1]);

    }

    void GetTwist(TwistMsg cmdVel)
    {
        rosLinear = (float)cmdVel.linear.x;
        rosAngular = (float)cmdVel.angular.z;
        lastCmdReceived = Time.time;
    }

    private void SetParameters(ArticulationBody joint)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.damping = dampingBase;
        joint.xDrive = drive;
    }

    private void SetSpeed(ArticulationBody joint, float wheelSpeed = float.NaN)
    {
        ArticulationDrive drive = joint.xDrive;
        if (float.IsNaN(wheelSpeed))
        {
            drive.targetVelocity = ((2 * maxLinearSpeed) / wheelRadius) * Mathf.Rad2Deg * (int)direction;
        }
        else
        {
            drive.targetVelocity = wheelSpeed;
        }
        joint.xDrive = drive;
    }

    private void ROSUpdate()
    {
        if (Time.time - lastCmdReceived > ROSTimeout)
        {
            rosLinear = 0f;
            rosAngular = 0f;
        }
        RobotInput(rosLinear, -rosAngular);
    }

    private void RobotInput(float speed, float rotSpeed) // m/s and rad/s
    {
        if (speed > maxLinearSpeed)
        {
            speed = maxLinearSpeed;
        }
        if (rotSpeed > maxRotationalSpeed)
        {
            rotSpeed = maxRotationalSpeed;
        }
        float wheel1Rotation = (speed / wheelRadius);
        float wheel2Rotation = wheel1Rotation;
        float wheelSpeedDiff = ((rotSpeed * trackWidth) / wheelRadius);
        if (rotSpeed != 0)
        {
            wheel1Rotation = (wheel1Rotation + (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
            wheel2Rotation = (wheel2Rotation - (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
        }
        else
        {
            wheel1Rotation *= Mathf.Rad2Deg;
            wheel2Rotation *= Mathf.Rad2Deg;
        }
        SetSpeed(articulationBodies[0], wheel1Rotation);
        SetSpeed(articulationBodies[1], wheel2Rotation);
    }

    private void FixedUpdate()
    {
        if (isConnected)
        {
            ROSUpdate();
        }
    }

    public void UnSub()
    {
        ros.Unsubscribe(topicName);
    }
}