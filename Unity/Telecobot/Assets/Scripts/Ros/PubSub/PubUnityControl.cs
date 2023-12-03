using RosMessageTypes.Geometry;
using RosMessageTypes.TelecobotRosUnity;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class PubUnityControl : MonoBehaviour
{
    private static readonly string ControlTopic = "/unity_control";

    public SceneMaster sceneMaster;

    private ROSConnection ros;
    private TelecobotUnityControlMsg controlMsg;

    [SerializeField]
    double m_PublishRateHz = 30f;
    double m_LastPublishTimeSeconds;
    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;
    bool ShouldPublishMessage => RosClock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    Vector2 b_aN => new Vector2(sceneMaster.rosConnector.arm_base_link.transform.position.x - sceneMaster.rosConnector.base_link.transform.position.x, sceneMaster.rosConnector.arm_base_link.transform.position.z - sceneMaster.rosConnector.base_link.transform.position.z).normalized;
    Vector2 b_ee => new Vector2(sceneMaster.rosConnector.endEffector.transform.position.x - sceneMaster.rosConnector.base_link.transform.position.x, sceneMaster.rosConnector.endEffector.transform.position.z - sceneMaster.rosConnector.base_link.transform.position.z);
    Vector2 b_eg => new Vector2(sceneMaster.uIMng.eeGripper.transform.position.x - sceneMaster.rosConnector.base_link.transform.position.x, sceneMaster.uIMng.eeGripper.transform.position.z - sceneMaster.rosConnector.base_link.transform.position.z);
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
    //両モード共通のコマンドを確認
    public void PublishCmd()
    {
        // Check the speed_cmd
        if (sceneMaster.inputMng.speedInc)
            controlMsg.speed_cmd = TelecobotUnityControlMsg.SPEED_INC;
        else if (sceneMaster.inputMng.speedDec)
            controlMsg.speed_cmd = TelecobotUnityControlMsg.SPEED_DEC;
        else controlMsg.speed_cmd = 0;

        // Check the speed_toggle_cmd
        if (sceneMaster.inputMng.speedCourse)
            controlMsg.speed_toggle_cmd = TelecobotUnityControlMsg.SPEED_COURSE;
        else if (sceneMaster.inputMng.speedFine)
            controlMsg.speed_toggle_cmd = TelecobotUnityControlMsg.SPEED_FINE;
        else controlMsg.speed_toggle_cmd = 0;

        //Check the pan_cmd
        if (sceneMaster.inputMng.pan >= 0.5)
            controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_CW;
        else if (sceneMaster.inputMng.pan <= -0.5)
            controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_CCW;
        else controlMsg.pan_cmd = 0;

        // Check the tilt_cmd
        if (sceneMaster.inputMng.tilt >= 0.5)
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.TILT_UP;
        else if (sceneMaster.inputMng.tilt <= -0.5)
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.TILT_DOWN;
        else controlMsg.tilt_cmd = 0;

        // Check if the camera pan-and-tilt mechanism should be reset
        if (sceneMaster.inputMng.panTiltHome)
        {
            controlMsg.pan_cmd = TelecobotUnityControlMsg.PAN_TILT_HOME;
            controlMsg.tilt_cmd = TelecobotUnityControlMsg.PAN_TILT_HOME;
        }

        // Check the gripper_cmd
        if (sceneMaster.inputMng.gripperClose)
            controlMsg.gripper_cmd = TelecobotUnityControlMsg.GRIPPER_CLOSE;
        else if (sceneMaster.inputMng.gripperOpen)
            controlMsg.gripper_cmd = TelecobotUnityControlMsg.GRIPPER_OPEN;
        else controlMsg.gripper_cmd = 0;

        // Check the gripper_pwm_cmd
        if (sceneMaster.inputMng.gripperPwmInc)
            controlMsg.gripper_pwm_cmd = TelecobotUnityControlMsg.GRIPPER_PWM_INC;
        else if (sceneMaster.inputMng.gripperPwmDec)
            controlMsg.gripper_pwm_cmd = TelecobotUnityControlMsg.GRIPPER_PWM_DEC;
        else controlMsg.gripper_pwm_cmd = 0;

        // Check the reboot_cmd
        if (sceneMaster.inputMng.rebootError)
            controlMsg.reboot_cmd = TelecobotUnityControlMsg.REBOOT_ERROR_MOTOR;
        else if (sceneMaster.inputMng.rebootAll)
            controlMsg.reboot_cmd = TelecobotUnityControlMsg.REBOOT_ALL_MOTOR;
        else controlMsg.reboot_cmd = 0;

        // Check the motor_cmd
        if (sceneMaster.inputMng.torqueEnable)
            controlMsg.motor_cmd = TelecobotUnityControlMsg.ENABLE_TORQUE;
        else if (sceneMaster.inputMng.torqueDisable)
            controlMsg.motor_cmd = TelecobotUnityControlMsg.DISABLE_TORQUE;
        else controlMsg.motor_cmd = 0;

        // Check the pose_cmd
        if (sceneMaster.inputMng.armHomePose)
            controlMsg.pose_cmd = TelecobotUnityControlMsg.HOME_POSE;
        else if (sceneMaster.inputMng.armSleepPose)
            controlMsg.pose_cmd = TelecobotUnityControlMsg.SLEEP_POSE;
        else controlMsg.pose_cmd = 0;
    }

    //マニュアルモードのコマンドを確認
    void PublishManualCmd()
    {
        // Check the base_x_cmd
        controlMsg.base_x_cmd = sceneMaster.inputMng.baseX * MAX_BASE_X;

        // Check the base_theta_cmd
        controlMsg.base_theta_cmd = -1.0 * sceneMaster.inputMng.baseRotate * MAX_BASE_THETA;

        // Check the ee_z_cmd
        if (sceneMaster.inputMng.eeZ >= 0.5)
            controlMsg.ee_z_cmd = TelecobotUnityControlMsg.EE_Z_INC;
        else if (sceneMaster.inputMng.eeZ <= -0.5)
            controlMsg.ee_z_cmd = TelecobotUnityControlMsg.EE_Z_DEC;
        else controlMsg.ee_z_cmd = 0;

        // Check the ee_x_cmd
        if (sceneMaster.inputMng.eeX >= 0.5)
            controlMsg.ee_x_cmd = TelecobotUnityControlMsg.EE_X_INC;
        else if (sceneMaster.inputMng.eeX <= -0.5)
            controlMsg.ee_x_cmd = TelecobotUnityControlMsg.EE_X_DEC;
        else controlMsg.ee_x_cmd = 0;

        // Check the ee_roll_cmd
        if (sceneMaster.inputMng.eeRoll >= 0.5)
            controlMsg.ee_roll_cmd = TelecobotUnityControlMsg.EE_ROLL_CW;
        else if (sceneMaster.inputMng.eeRoll <= -0.5)
            controlMsg.ee_roll_cmd = TelecobotUnityControlMsg.EE_ROLL_CCW;
        else controlMsg.ee_roll_cmd = 0;

        // Check the ee_pitch_cmd
        if (sceneMaster.inputMng.eePitch >= 0.5)
            controlMsg.ee_pitch_cmd = TelecobotUnityControlMsg.EE_PITCH_UP;
        else if (sceneMaster.inputMng.eePitch <= -0.5)
            controlMsg.ee_pitch_cmd = TelecobotUnityControlMsg.EE_PITCH_DOWN;
        else controlMsg.ee_pitch_cmd = 0;

        // Check the waist_cmd
        if (sceneMaster.inputMng.waistRotate >= 0.5)
            controlMsg.waist_cmd = TelecobotUnityControlMsg.WAIST_CW;
        else if (sceneMaster.inputMng.waistRotate <= -0.5)
            controlMsg.waist_cmd = TelecobotUnityControlMsg.WAIST_CCW;
        else controlMsg.waist_cmd = 0;
    }

    //セミオートモードのコマンドを確認
    void PublishSemiAutoCmd()
    {
        // Check the pose_cmd
        if (sceneMaster.inputMng.semiAutoCmd == SemiAutomaticCommands.Available)
        {
            controlMsg.base_cmd = 0;
            controlMsg.arm_cmd = 0;
        }
        else if (sceneMaster.inputMng.semiAutoCmd == SemiAutomaticCommands.Disable)
        {
            controlMsg.base_cmd = 0;
            controlMsg.arm_cmd = 0;
            controlMsg.pose_data = null;
            controlMsg.end_effector_pose = null;
            controlMsg.goal_pose = null;
        }
        else if (sceneMaster.inputMng.semiAutoCmd == SemiAutomaticCommands.PlaceGoal)
        {
            if(sceneMaster.inputMng.moveBase)
            {
                Debug.LogWarning("Place Goal called.");
                sceneMaster.uIMng.CreateOrResetGoal();
            }
        }
        else if (sceneMaster.inputMng.semiAutoCmd == SemiAutomaticCommands.BackHome)
        {
            Debug.LogWarning("Back home method is not implemented.");
        }
        else if (sceneMaster.inputMng.semiAutoCmd == SemiAutomaticCommands.PublishGoal)
        {
            controlMsg.base_cmd = TelecobotUnityControlMsg.MOVE_BASE;
            Debug.LogWarning("Check goal is called.");
            sceneMaster.uIMng.CheckGoal();
        }
        else if (sceneMaster.inputMng.semiAutoCmd == SemiAutomaticCommands.PlaceTarget)
        {
            if(sceneMaster.inputMng.moveArm)
            {
                sceneMaster.uIMng.CreateOrResetTarget();
            }
        }
        else if (sceneMaster.inputMng.semiAutoCmd == SemiAutomaticCommands.PublishTarget)
        {
            //controlMsg.arm_cmd = TelecobotUnityControlMsg.MOVEIT;
            controlMsg.arm_cmd = TelecobotUnityControlMsg.MOVE_ARM;
            sceneMaster.uIMng.PickTarget();
        }
        else Debug.LogWarning("Unknown commands. somting went wrong.");
    }

    //以下、cmd以外のPubする数値を設定する関数
    //LocobotBase(Create3)
    public void SetMoveToPose()
    {
        controlMsg.pose_data = new double[3];
        controlMsg.pose_data[0] = sceneMaster.uIMng.goal.transform.position.z; //x
        controlMsg.pose_data[1] = -sceneMaster.uIMng.goal.transform.position.x; //y
        controlMsg.pose_data[2] = (Mathf.Repeat(sceneMaster.uIMng.goal.transform.rotation.y + 180, 360) - 180) * Mathf.Deg2Rad; //Yaw
    }

    //LocobotArm
    public void SetTargetPose()
    {
        //Eeの変化量をPub
        var rEeG = sceneMaster.rosConnector.base_link.transform.InverseTransformPoint(sceneMaster.uIMng.eeGripper.transform.position);
        var rEe = sceneMaster.rosConnector.base_link.transform.InverseTransformPoint(sceneMaster.rosConnector.endEffector.transform.position);
        var diff = rEeG - rEe;
        controlMsg.pose_data = new double[5];
        controlMsg.pose_data[0] = diff.z;//rosX
        controlMsg.pose_data[1] = -diff.x;//rosY
        controlMsg.pose_data[2] = diff.y;//rosZ

        var displacementAngle = sceneMaster.uIMng.eeGripper.transform.eulerAngles - sceneMaster.rosConnector.endEffector.transform.eulerAngles;
        //-180~180に正規化
        var rotX = (Mathf.Repeat(displacementAngle.x + 180, 360) - 180);//pitch-rosY
        var rotY = (Mathf.Repeat(displacementAngle.y + 180, 360) - 180);//yaw-rosZ
        var rotZ = (Mathf.Repeat(displacementAngle.z + 180, 360) - 180);//roll-rosX
        //度をラジアンに変換しPub
        controlMsg.pose_data[3] = Mathf.Deg2Rad * -rotZ;
        controlMsg.pose_data[4] = Mathf.Deg2Rad * rotX;
        //controlMsg.pose_data[5] = Mathf.Deg2Rad * rotY;
    }

    //Moveitを使用する際はこの関数を呼ぶ。SetTargetPose()を参照している部分を書き換える。
    public void SetMoveitPose()
    {
        controlMsg.goal_pose = new PoseMsg
        {
            position = sceneMaster.uIMng.eeGripper.transform.localPosition.To<FLU>(),
            orientation = Quaternion.Euler(90, sceneMaster.uIMng.eeGripper.transform.eulerAngles.y, 0).To<FLU>()
        };

        // エンドエフェクタの位置を格納
        controlMsg.end_effector_pose = new PoseMsg
        {
            position = sceneMaster.rosConnector.endEffector.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(sceneMaster.rosConnector.endEffector.transform.eulerAngles.x,
                                            sceneMaster.rosConnector.endEffector.transform.eulerAngles.y,
                                            sceneMaster.rosConnector.endEffector.transform.eulerAngles.z).To<FLU>()
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneMaster.rosConnector.rosConnection==RosConnection.Connect&&sceneMaster.inputMng!=null)
        {
            if (sceneMaster.userSettings.role == Role.Robot)
            {
                if (ShouldPublishMessage)
                {
                    //共通操作のコマンド変化をチェック
                    PublishCmd();
                    //マニュアル操作の場合のコマンド変化をチェック
                    if (sceneMaster.inputMng.controlMode == ControlMode.ManualControl)
                    {
                        PublishManualCmd();
                    }
                    //セミオート操作の場合のコマンド変化をチェック(ゴール・ターゲット位置の確定時のみPub)
                    else if (sceneMaster.inputMng.controlMode == ControlMode.SemiAutomaticControl)
                    {
                        PublishSemiAutoCmd();
                    }
                    // After checking all cmds publish all msg to /unity_control topic.
                    ros.Publish(ControlTopic, controlMsg);
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (sceneMaster.rosConnector.rosConnection == RosConnection.Connect)
        {
            for(int i = 0; i< 5; i++)
            {
            controlMsg.pose_cmd = TelecobotUnityControlMsg.SLEEP_POSE;
            ros.Publish(ControlTopic, controlMsg);
            }
        }
    }
}
