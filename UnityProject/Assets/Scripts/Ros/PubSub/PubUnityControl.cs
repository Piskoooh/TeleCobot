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

    //LocobotPCが直接通信しているJoy(PS4のコントローラ)では更新スピードを変更できるようになっているが、
    //Unity側の実装はボタン配置を変更する関係で更新スピードの変更ボタンを廃止。
    //更新スピードを25Hzで固定
    //確認していないけどPS4のコントローラでロボット側のみの更新スピードを変更すると挙動が変になる可能性あり。
    //ロボット側は再起動すればデフォの25Hzに戻る。
    double m_PublishRateHz = 25f;
    double m_LastPublishTimeSeconds;
    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;
    bool ShouldPublishMessage => RosClock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    private ROSConnection ros;
    private TelecobotUnityControlMsg controlMsg;


    private bool isConnected = false;
    bool flag;

    static double MAX_BASE_X = 0.7;         // Max translational motion that the base can do is 0.7 m/s
    static double MAX_BASE_THETA = 3.14;    // Max rotational motion that the base can do is 3.14 rad/s
    double threshold;                                           // Joystick sensitivity threshold
    //static bool R_button_pressed = false;
    //static bool L_button_pressed = false;
    static bool switch_cmd = false;
    static bool switch_cmd_last_state = false;

    // Start is called before the first frame update
    void Start()
    {
        isConnected = false;
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
        isConnected = true;
        m_LastPublishTimeSeconds = RosClock.time + PublishPeriodSeconds;
    }

    public void OnRosDisconnected()
    {
        isConnected = false;
    }

    //連続でPubしたいメッセージ
    public void PublishJoy()
    {
        // Check the speed_cmd
        if (inputMng.speedInc)
            controlMsg.speed_cmd = LocobotJoyMsg.SPEED_INC;
        else if (inputMng.speedDec)
            controlMsg.speed_cmd = LocobotJoyMsg.SPEED_DEC;
        else controlMsg.speed_cmd = 0;

        // Check the speed_toggle_cmd
        if (inputMng.speedCourse)
            controlMsg.speed_toggle_cmd = LocobotJoyMsg.SPEED_COURSE;
        else if (inputMng.speedFine)
            controlMsg.speed_toggle_cmd = LocobotJoyMsg.SPEED_FINE;
        else controlMsg.speed_toggle_cmd = 0;

        // Check the base_x_cmd
        controlMsg.base_x_cmd = inputMng.baseX * MAX_BASE_X;

        // Check for the first time the R or L buttons are pressed
        // This is necessary to fix a potential bug in the 'joy' package - refer to https://github.com/ros-drivers/joystick_drivers/issues/155
        //if (inputMng.baseRotate>0 && R_button_pressed == false)
        //    R_button_pressed = true;
        //if (inputMng.baseRotate<0 && L_button_pressed == false)
        //    L_button_pressed = true;

        // Check the base_theta_cmd
        //if (L_button_pressed && R_button_pressed)
            controlMsg.base_theta_cmd = inputMng.baseRotate / 2.0 * MAX_BASE_THETA;
        //else if (L_button_pressed && !R_button_pressed)
        //    controlMsg.base_theta_cmd = (1.0 - inputMng.baseRotate) / 2.0 * MAX_BASE_THETA;
        //else if (!L_button_pressed && R_button_pressed)
        //    controlMsg.base_theta_cmd = (inputMng.baseRotate - 1.0) / 2.0 * MAX_BASE_THETA;

        // Check the pan_cmd
        if (inputMng.pan > 0)
            controlMsg.pan_cmd = LocobotJoyMsg.PAN_CCW;
        else if (inputMng.pan < 0)
            controlMsg.pan_cmd = LocobotJoyMsg.PAN_CW;
        else controlMsg.pan_cmd = 0;

        // Check the tilt_cmd
        if (inputMng.tilt > 0)
            controlMsg.tilt_cmd = LocobotJoyMsg.TILT_DOWN;
        else if (inputMng.tilt < 0)
            controlMsg.tilt_cmd = LocobotJoyMsg.TILT_UP;
        else controlMsg.tilt_cmd = 0;

        // Check if the camera pan-and-tilt mechanism should be reset
        if (inputMng.panTiltHome)
        {
            controlMsg.pan_cmd = LocobotJoyMsg.PAN_TILT_HOME;
            controlMsg.tilt_cmd = LocobotJoyMsg.PAN_TILT_HOME;
        }

        // Check the ee_y_cmd
        if (inputMng.eeY > 0)
            controlMsg.ee_y_cmd = LocobotJoyMsg.EE_Y_INC;
        else if (inputMng.eeY < 0)
            controlMsg.ee_y_cmd = LocobotJoyMsg.EE_Y_DEC;
        else controlMsg.ee_z_cmd = 0;

        // Check the ee_z_cmd
        if (inputMng.eeZ > 0)
            controlMsg.ee_z_cmd = LocobotJoyMsg.EE_Z_INC;
        else if (inputMng.eeZ < 0)
            controlMsg.ee_z_cmd = LocobotJoyMsg.EE_Z_DEC;
        else controlMsg.ee_z_cmd = 0;

        // Check the ee_roll_cmd
        if (inputMng.eeRoll > 0)
            controlMsg.ee_roll_cmd = LocobotJoyMsg.EE_ROLL_CW;
        else if (inputMng.eeRoll < 0)
            controlMsg.ee_roll_cmd = LocobotJoyMsg.EE_ROLL_CCW;
        else controlMsg.ee_roll_cmd = 0;

        // Check the ee_pitch_cmd
        if (inputMng.eePitch > 0)
            controlMsg.ee_pitch_cmd = LocobotJoyMsg.EE_PITCH_UP;
        else if (inputMng.eePitch < 0)
            controlMsg.ee_pitch_cmd = LocobotJoyMsg.EE_PITCH_DOWN;
        else controlMsg.ee_pitch_cmd=0;

        // Check the waist_cmd
        if (inputMng.waistRotate>0)
            controlMsg.waist_cmd = LocobotJoyMsg.WAIST_CCW;
        else if (inputMng.waistRotate<0)
            controlMsg.waist_cmd = LocobotJoyMsg.WAIST_CW;
        else controlMsg.waist_cmd = 0;

        // Check the gripper_cmd
        if (inputMng.gripperClose)
            controlMsg.gripper_cmd = LocobotJoyMsg.GRIPPER_CLOSE;
        else if (inputMng.gripperOpen)
            controlMsg.gripper_cmd = LocobotJoyMsg.GRIPPER_OPEN;
        else controlMsg.gripper_cmd = 0;

        // Check the pose_cmd
        if (inputMng.armHomePose)
            controlMsg.pose_cmd = LocobotJoyMsg.HOME_POSE;
        else if (inputMng.armSleepPose)
            controlMsg.pose_cmd = LocobotJoyMsg.SLEEP_POSE;
        else controlMsg.pose_cmd = 0;

        // Check the gripper_pwm_cmd
        if (inputMng.gripperPwm > 0)
            controlMsg.gripper_pwm_cmd = LocobotJoyMsg.GRIPPER_PWM_INC;
        else if (inputMng.gripperPwm < 0)
            controlMsg.gripper_pwm_cmd = LocobotJoyMsg.GRIPPER_PWM_DEC;
        else controlMsg.gripper_pwm_cmd = 0;

        //Debug.LogWarning("PublishedJoyMsg!!");
        ros.Publish(ControlTopic, controlMsg);
        
        m_LastPublishTimeSeconds = RosClock.FrameStartTimeInSeconds;
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
        ros.Publish(ControlTopic, controlMsg);
    }

    //LocobotCore
    public void PubTorqueEnable()
    {
        ros.Publish(ControlTopic, controlMsg);
    }

    public void PubTorqueDisable()
    {
        ros.Publish(ControlTopic, controlMsg);
    }

    public void PubSmartMotorReboot()
    {
        ros.Publish(ControlTopic, controlMsg);
    }

    public void PubAllMotorReboot()
    {
        ros.Publish(ControlTopic, controlMsg);
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected)
        {
            if (ShouldPublishMessage)
            {
                PublishJoy();
            }
        }
    }
}
