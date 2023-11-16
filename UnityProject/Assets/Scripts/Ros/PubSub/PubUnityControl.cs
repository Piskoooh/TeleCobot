using RosMessageTypes.Geometry;
using RosMessageTypes.TelecobotRosUnity;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class PubUnityControl : MonoBehaviour
{
    private static readonly string ControlTopic = "/unity_control";
    public GameObject endEffector;
    public GameObject base_link;
    public GameObject arm_base_link;

    public InputManager inputMng;
    public UIManager uIMng;

    private ROSConnection ros;
    private TelecobotUnityControlMsg controlMsg;
    [SerializeField]
    private RosConnector rosConnector;

    [SerializeField]
    double m_PublishRateHz = 30f;
    double m_LastPublishTimeSeconds;
    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;
    bool ShouldPublishMessage => RosClock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

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

    //コントローラなどの入力デバイスからの入力内容に応じてPubするメッセージを変更
    public void PublishCmd()
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

        // Check the pan_cmd
        //if (inputMng.pan >= 0.5)
        //    controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_CCW;
        //else if (inputMng.pan <= 0.5)
        //    controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_CW;
        //else controlMsg.pan_cmd = 0;

        // Check the tilt_cmd
        if (inputMng.tilt >= 0.5)
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.TILT_DOWN;
        else if (inputMng.tilt <= -0.5)
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.TILT_UP;
        else controlMsg.tilt_cmd = 0;

        // Check if the camera pan-and-tilt mechanism should be reset
        if (inputMng.panTiltHome)
        {
            controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_TILT_HOME;
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.PAN_TILT_HOME;
        }

        // Check the gripper_cmd
        if (inputMng.gripperClose)
            controlMsg.gripper_cmd = TelecobotUnityControlMsg.GRIPPER_CLOSE;
        else if (inputMng.gripperOpen)
            controlMsg.gripper_cmd = TelecobotUnityControlMsg.GRIPPER_OPEN;
        else controlMsg.gripper_cmd = 0;

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

        // Check the pose_cmd
        if (inputMng.armHomePose)
            controlMsg.pose_cmd = TelecobotUnityControlMsg.HOME_POSE;
        else if (inputMng.armSleepPose)
            controlMsg.pose_cmd = TelecobotUnityControlMsg.SLEEP_POSE;
        else controlMsg.pose_cmd = 0;
    }

    void PublishManuCmd()
    {
        // Check the base_x_cmd
        controlMsg.base_x_cmd = inputMng.baseX * MAX_BASE_X;

        // Check the base_theta_cmd
        controlMsg.base_theta_cmd = -1.0 * inputMng.baseRotate * MAX_BASE_THETA;

        // Check the ee_z_cmd
        if (inputMng.eeZ >= 0.5)
            controlMsg.ee_z_cmd = TelecobotUnityControlMsg.EE_Z_INC;
        else if (inputMng.eeZ <= -0.5)
            controlMsg.ee_z_cmd = TelecobotUnityControlMsg.EE_Z_DEC;
        else controlMsg.ee_z_cmd = 0;

        // Check the ee_x_cmd
        if (inputMng.eeX >= 0.5)
            controlMsg.ee_x_cmd = TelecobotUnityControlMsg.EE_X_INC;
        else if (inputMng.eeX <= -0.5)
            controlMsg.ee_x_cmd = TelecobotUnityControlMsg.EE_X_DEC;
        else controlMsg.ee_x_cmd = 0;

        // Check the ee_roll_cmd
        if (inputMng.eeRoll >= 0.5)
            controlMsg.ee_roll_cmd = TelecobotUnityControlMsg.EE_ROLL_CCW;
        else if (inputMng.eeRoll <= -0.5)
            controlMsg.ee_roll_cmd = TelecobotUnityControlMsg.EE_ROLL_CW;
        else controlMsg.ee_roll_cmd = 0;

        // Check the ee_pitch_cmd
        if (inputMng.eePitch >= 0.5)
            controlMsg.ee_pitch_cmd = TelecobotUnityControlMsg.EE_PITCH_DOWN;
        else if (inputMng.eePitch <= -0.5)
            controlMsg.ee_pitch_cmd = TelecobotUnityControlMsg.EE_PITCH_UP;
        else controlMsg.ee_pitch_cmd = 0;

        // Check the waist_cmd
        if (inputMng.waistRotate >= 0.5)
            controlMsg.waist_cmd = TelecobotUnityControlMsg.WAIST_CW;
        else if (inputMng.waistRotate <= -0.5)
            controlMsg.waist_cmd = TelecobotUnityControlMsg.WAIST_CCW;
        else controlMsg.waist_cmd = 0;
    }

    void PublishSemiAutoCmd()
    {
        // Check the pose_cmd
        if (inputMng.semiAutoCmd == SemiAutomaticCommands.Home)
        {
            controlMsg.pose_cmd = TelecobotUnityControlMsg.HOME_POSE;
            controlMsg.base_cmd = 0;
            controlMsg.arm_cmd = 0;
        }
        else if (inputMng.semiAutoCmd == SemiAutomaticCommands.Sleep)
        {
            controlMsg.pose_cmd = TelecobotUnityControlMsg.SLEEP_POSE;
            controlMsg.base_cmd = 0;
            controlMsg.arm_cmd = 0;
        }
        else if (inputMng.semiAutoCmd == SemiAutomaticCommands.Available)
        {
            controlMsg.pose_cmd = 0;
            controlMsg.base_cmd = 0;
            controlMsg.arm_cmd = 0;
        }
        else if (inputMng.semiAutoCmd == SemiAutomaticCommands.Disable)
        {
            controlMsg.pose_cmd = 0;
            controlMsg.base_cmd = 0;
            controlMsg.arm_cmd = 0;
            controlMsg.pose_data = null;
            controlMsg.end_effector_pose = null;
            controlMsg.goal_pose = null;
        }
        //else Debug.LogWarning("publish method or unknown method.");

        else if (inputMng.semiAutoCmd == SemiAutomaticCommands.PlaceGoal)
        {
            if(inputMng.moveBase)
            {
                Debug.LogWarning("Place Goal called but not implemented.");
            }
        }
        else if (inputMng.semiAutoCmd == SemiAutomaticCommands.BackHome)
        {
            Debug.LogWarning("Back home method is not implemented.");
        }
        else if (inputMng.semiAutoCmd == SemiAutomaticCommands.PlaceTarget)
        {
            if(inputMng.moveArm)
            {
                uIMng.CreateOrResetTarget();
                if (uIMng.visualIndicator == null)
                {
                    uIMng.VisualRange();
                }
            }
        }

        // Check the base_cmd or arm_cmd
        else if (inputMng.semiAutoCmd == SemiAutomaticCommands.PublishGoal&&inputMng.setGoalOrTarget)
        {
            controlMsg.base_cmd = TelecobotUnityControlMsg.MOVE_BASE;
            //controlMsg.pose_data=(x,y,yaw)
            Debug.LogWarning("Method for publishing goal pose data is not implemented.");
        }
        else if (inputMng.semiAutoCmd == SemiAutomaticCommands.PublishTarget&&inputMng.setGoalOrTarget)
        {
            //controlMsg.arm_cmd = TelecobotUnityControlMsg.MOVEIT;
            controlMsg.arm_cmd = TelecobotUnityControlMsg.SET_EE_CARTESIAN_TRAJECTORY;
            uIMng.PickTarget();
        }
        else Debug.LogWarning("Unknown commands. somting went wrong.");
    }

    //LocobotBase(Create3)
    public void PubMoveToPose()
    {

    }

    //LocobotArm
    public void PubTargetPose()
    {
        //Eeの変化量をPub
        var displacementPose = uIMng.eeGripper.transform.position - endEffector.transform.position;
        var displacementAngle = uIMng.eeGripper.transform.eulerAngles - endEffector.transform.eulerAngles;
        controlMsg.pose_data=new double[5];
        controlMsg.pose_data[0] = displacementPose.z;//rosX
        controlMsg.pose_data[1] = 0;//rosY
        controlMsg.pose_data[2] = displacementPose.y;//rosZ
        //-180~180に正規化
        var rotZ = (Mathf.Repeat(displacementAngle.z + 180, 360) - 180);//roll-rosX
        var rotX = (Mathf.Repeat(displacementAngle.x + 180, 360) - 180);//pitch-rosY
        var rotY = (Mathf.Repeat(displacementAngle.y + 180, 360) - 180);//yaw-rosZ
        //度をラジアンに変換しPub
        controlMsg.pose_data[3] = Mathf.Deg2Rad * -rotZ;
        controlMsg.pose_data[4] = Mathf.Deg2Rad * rotX;
        //controlMsg.pose_data[5] = Mathf.Deg2Rad * rotY;
    }
    public void PubMoveitPose()
    {
        controlMsg.goal_pose = new PoseMsg
        {
            position = uIMng.eeGripper.transform.localPosition.To<FLU>(),
            orientation = Quaternion.Euler(90, uIMng.eeGripper.transform.eulerAngles.y, 0).To<FLU>()
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
            //if (ShouldPublishMessage)
            {
                //共通操作のコマンド変化をチェック
                PublishCmd();
                //マニュアル操作の場合のコマンド変化をチェック
                if (inputMng.controlMode == ControlMode.ManualControl)
                {
                    PublishManuCmd();
                }
                //セミオート操作の場合のコマンド変化をチェック(ゴール・ターゲット位置の確定時のみPub)
                else if (inputMng.controlMode == ControlMode.SemiAutomaticControl)
                {
                    PublishSemiAutoCmd();
                }

                // After checking all cmds publish all msg to /unity_control topic.
                //Debug.LogWarning("PublishedJoyMsg!!");
                ros.Publish(ControlTopic, controlMsg);
            }
        }
    }
}
