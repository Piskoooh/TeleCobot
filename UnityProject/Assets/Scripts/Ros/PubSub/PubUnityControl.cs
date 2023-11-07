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

    //ispressedで反応するようにする
    //連続でPubしたいメッセージ
    public void PublishJoy()
    {
        //arm
        // Check the pose_cmd
        if (inputMng.armHomePose)
            controlMsg.pose_cmd = LocobotJoyMsg.HOME_POSE;
        else if (inputMng.armSleepPose)
            controlMsg.pose_cmd = LocobotJoyMsg.SLEEP_POSE;
        else controlMsg.pose_cmd = 0;


        //turret

        //base

        //gripper

        //if (prevJoyMsg != controlMsg)
        
            Debug.LogWarning("PublishedJoyMsg!!");
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
