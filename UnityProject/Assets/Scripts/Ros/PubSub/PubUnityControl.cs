using RosMessageTypes.Geometry;
using RosMessageTypes.TelecobotRosUnity;
using RosMessageTypes.InterbotixXs;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class PubUnityControl : MonoBehaviour
{
    private static readonly string ControlTopic = "/unity_control";
    public GameObject endEffector;
    public GameObject arm_base_link;

    public InputManager inputMng;

    private ROSConnection ros;
    private TelecobotUnityControlMsg controlMsg;
    [SerializeField]
    private RosConnector rosConnector;

    bool flag;

    static double MAX_BASE_X = 0.7;         // Max translational motion that the base can do is 0.7 m/s
    static double MAX_BASE_THETA = 3.14;    // Max rotational motion that the base can do is 3.14 rad/s

    // Start is called before the first frame update
    void Start()
    {
        flag = false;
    }

    public void OnRosConnect()
    {
        ros = ROSConnection.GetOrCreateInstance();
        if (flag == false)
        {
            ros.RegisterPublisher<TelecobotUnityControlMsg>(ControlTopic);
            flag = true;
        }
        controlMsg = new TelecobotUnityControlMsg();
    }

    public void OnRosDisconnected()
    {

    }

    //連続でPubしたいメッセージ
    public void PublishJoy()
    {
        // Check the speed_cmd
        if (inputMng.speedInc)
            controlMsg.speed_cmd = TelecobotUnityControlMsg.SPEED_INC;
        else if (inputMng.speedDec)
            controlMsg.speed_cmd = TelecobotUnityControlMsg.SPEED_DEC;
        else controlMsg.speed_cmd = 0;

        // Check the speed_toggle_cmd
        if (inputMng.speedCourse)
            controlMsg.speed_toggle_cmd = TelecobotUnityControlMsg.SPEED_COURSE;
        else if (inputMng.speedFine)
            controlMsg.speed_toggle_cmd = TelecobotUnityControlMsg.SPEED_FINE;
        else controlMsg.speed_toggle_cmd = 0;

        // Check the base_x_cmd
        controlMsg.base_x_cmd = inputMng.baseX * MAX_BASE_X;

        // Check the base_theta_cmd
        controlMsg.base_theta_cmd = -1.0 * inputMng.baseRotate * MAX_BASE_THETA;

        // Check the pan_cmd
        //if (inputMng.pan > 0)
        //    controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_CCW;
        //else if (inputMng.pan < 0)
        //    controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_CW;
        //else controlMsg.pan_cmd = 0;

        // Check the tilt_cmd
        if (inputMng.tilt > 0)
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.TILT_DOWN;
        else if (inputMng.tilt < 0)
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.TILT_UP;
        else controlMsg.tilt_cmd = 0;

        // Check if the camera pan-and-tilt mechanism should be reset
        if (inputMng.panTiltHome)
        {
            controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_TILT_HOME;
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.PAN_TILT_HOME;
        }

        // Check the ee_z_cmd
        if (inputMng.eeZ > 0)
            controlMsg.ee_z_cmd = TelecobotUnityControlMsg.EE_Z_INC;
        else if (inputMng.eeZ < 0)
            controlMsg.ee_z_cmd = TelecobotUnityControlMsg.EE_Z_DEC;
        else controlMsg.ee_z_cmd = 0;

        // Check the ee_x_cmd
        if (inputMng.eeX > 0)
            controlMsg.ee_x_cmd = TelecobotUnityControlMsg.EE_X_INC;
        else if (inputMng.eeX < 0)
            controlMsg.ee_x_cmd = TelecobotUnityControlMsg.EE_X_DEC;
        else controlMsg.ee_x_cmd = 0;

        // Check the ee_roll_cmd
        if (inputMng.eeRoll > 0)
            controlMsg.ee_roll_cmd = TelecobotUnityControlMsg.EE_ROLL_CCW;
        else if (inputMng.eeRoll < 0)
            controlMsg.ee_roll_cmd = TelecobotUnityControlMsg.EE_ROLL_CW;
        else controlMsg.ee_roll_cmd = 0;

        // Check the ee_pitch_cmd
        if (inputMng.eePitch > 0)
            controlMsg.ee_pitch_cmd = TelecobotUnityControlMsg.EE_PITCH_DOWN;
        else if (inputMng.eePitch < 0)
            controlMsg.ee_pitch_cmd = TelecobotUnityControlMsg.EE_PITCH_UP;
        else controlMsg.ee_pitch_cmd = 0;

        // Check the waist_cmd
        if (inputMng.waistRotate > 0)
            controlMsg.waist_cmd = TelecobotUnityControlMsg.WAIST_CW;
        else if (inputMng.waistRotate < 0)
            controlMsg.waist_cmd = TelecobotUnityControlMsg.WAIST_CCW;
        else controlMsg.waist_cmd = 0;

        // Check the gripper_cmd
        if (inputMng.gripperClose)
            controlMsg.gripper_cmd = TelecobotUnityControlMsg.GRIPPER_CLOSE;
        else if (inputMng.gripperOpen)
            controlMsg.gripper_cmd = TelecobotUnityControlMsg.GRIPPER_OPEN;
        else controlMsg.gripper_cmd = 0;

        // Check the pose_cmd
        if (inputMng.armHomePose)
            controlMsg.pose_cmd = TelecobotUnityControlMsg.HOME_POSE;
        else if (inputMng.armSleepPose)
            controlMsg.pose_cmd = TelecobotUnityControlMsg.SLEEP_POSE;
        else controlMsg.pose_cmd = 0;

        // Check the gripper_pwm_cmd
        if (inputMng.gripperPwmInc)
            controlMsg.gripper_pwm_cmd = TelecobotUnityControlMsg.GRIPPER_PWM_INC;
        else if (inputMng.gripperPwmDec)
            controlMsg.gripper_pwm_cmd = TelecobotUnityControlMsg.GRIPPER_PWM_DEC;
        else controlMsg.gripper_pwm_cmd = 0;

        // Check the reboot_cmd
        if (inputMng.rebootError)
            controlMsg.reboot_cmd = TelecobotUnityControlMsg.REBOOT_ERROR_MOTOR;
        else if (inputMng.rebootAll)
            controlMsg.reboot_cmd = TelecobotUnityControlMsg.REBOOT_ALL_MOTOR;
        else controlMsg.reboot_cmd = 0;

        // Check the motor_cmd
        if (inputMng.torqueEnable)
            controlMsg.motor_cmd = TelecobotUnityControlMsg.ENABLE_TORQUE;
        else if (inputMng.torqueDisable)
            controlMsg.motor_cmd = TelecobotUnityControlMsg.DISABLE_TORQUE;
        else controlMsg.motor_cmd = 0;

        //Debug.LogWarning("PublishedJoyMsg!!");
        ros.Publish(ControlTopic, controlMsg);
    }

    //LocobotBase(Create3)
    public void PubMoveToPose()
    {
        ros.Publish(ControlTopic, controlMsg);

    }

    //LocobotArm
    public void PubEeGoalPose()
    {
        ros.Publish(ControlTopic, controlMsg);
    }

    public void PubEeMoveitPose(Transform target)
    {
        controlMsg.goal_pose = new PoseMsg
        {
            position = target.localPosition.To<FLU>(),
            orientation = Quaternion.Euler(90, target.eulerAngles.y, 0).To<FLU>()
        };
        
        // エンドエフェクタの位置を格納
        controlMsg.end_effector_pose = new PoseMsg
        {
            position = endEffector.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(endEffector.transform.eulerAngles.x,
                                            endEffector.transform.eulerAngles.y,
                                            endEffector.transform.eulerAngles.z).To<FLU>()
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (rosConnector.rosConnection==RosConnection.Connect)
        {
            PublishJoy();
        }
    }
}
