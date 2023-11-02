using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Unity.Robotics.UrdfImporter.Control;
using UnityEngine.InputSystem;


public class SubCmdVel : MonoBehaviour
{
    public enum ControlMode { Keyboard, ROS };
    public ControlMode mode = ControlMode.ROS;


    public ArticulationBody left_wheel_joint;
    public ArticulationBody right_wheel_joint;

    private ArticulationBody[] articulationBodies;
    public string topicName = "/mobile_base/cmd_vel";
    private ROSConnection ros;

    public float maxLinearSpeed = 0.7f; //  m/s
    public float maxRotationalSpeed = 3.14f;//rad/s
    public float wheelRadius = 0.03575f; //meters
    public float trackWidth = 0.233f; // meters Distance between tyres
    public float linarSpeedCoefficient = 0f;
    public float angularSpeedCoefficient = 0f;

    public float ROSTimeout = 0.5f;
    private float lastCmdReceived = 0f;

    private RotationDirection direction;
    private float rosLinear = 0f;
    private float rosAngular = 0f;

    private bool isConnected = false;

    private void Start()
    {
        isConnected = false;
        if (mode == ControlMode.Keyboard)
        {
            ros = ROSConnection.GetOrCreateInstance();
            ros.Subscribe<TwistMsg>(topicName, GetTwist);
            articulationBodies = new ArticulationBody[2];
            articulationBodies[0] = left_wheel_joint;
            articulationBodies[1] = right_wheel_joint;
        }
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

    void SetParameters(ArticulationBody body)
    {
        ArticulationDrive drive = body.xDrive;
        drive.driveType = ArticulationDriveType.Velocity;
        body.xDrive = drive;
    }

    void GetTwist(TwistMsg cmdVel)
    {
        rosLinear = (float)cmdVel.linear.x;
        rosAngular = (float)cmdVel.angular.z;
        lastCmdReceived = Time.time;
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

    private void KeyBoardUpdate()
    {
        float moveDirection = Keyboard.current[Key.W].isPressed ? 1 :
                              Keyboard.current[Key.S].isPressed ? -1 :
                              Keyboard.current[Key.UpArrow].isPressed ? 1 :
                              Keyboard.current[Key.DownArrow].isPressed ? -1 : 0;
        float inputSpeed;
        float inputRotationSpeed;
        if (moveDirection > 0)
        {
            inputSpeed = maxLinearSpeed;
        }
        else if (moveDirection < 0)
        {
            inputSpeed = maxLinearSpeed * -1;
        }
        else
        {
            inputSpeed = 0;
        }

        float turnDirection = Keyboard.current[Key.D].isPressed ? 1 :
                              Keyboard.current[Key.A].isPressed ? -1 :
                              Keyboard.current[Key.RightArrow].isPressed ? 1 :
                              Keyboard.current[Key.LeftArrow].isPressed ? -1 : 0;
        if (turnDirection > 0)
        {
            inputRotationSpeed = maxRotationalSpeed;
        }
        else if (turnDirection < 0)
        {
            inputRotationSpeed = maxRotationalSpeed * -1;
        }
        else
        {
            inputRotationSpeed = 0;
        }
        RobotInput(inputSpeed, inputRotationSpeed);
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
        float wheelRotDiff = ((rotSpeed * trackWidth) / wheelRadius);
        //float wheelRotDiff = rotSpeed / (trackWidth * wheelRadius);
        if (rotSpeed != 0)
        {
            wheel1Rotation = (wheel1Rotation * linarSpeedCoefficient + wheelRotDiff * angularSpeedCoefficient) * Mathf.Rad2Deg;
            wheel2Rotation = (wheel2Rotation * linarSpeedCoefficient - wheelRotDiff * angularSpeedCoefficient) * Mathf.Rad2Deg;
        }
        else
        {
            wheel1Rotation = wheel1Rotation * Mathf.Rad2Deg * linarSpeedCoefficient;
            wheel2Rotation = wheel2Rotation * Mathf.Rad2Deg * linarSpeedCoefficient;
        }

        //float wheel1Rotation = speed/wheelRadius;
        SetSpeed(articulationBodies[0], wheel1Rotation);
        SetSpeed(articulationBodies[1], wheel2Rotation);
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

    private void FixedUpdate()
    {
        if (isConnected)
        {
            ROSUpdate();
        }
        else if(mode==ControlMode.Keyboard)
        {
            KeyBoardUpdate();
        }
    }

    public void UnSub()
    {
        ros.Unsubscribe(topicName);
    }
}