using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;

using RosMessageTypes.Geometry;
using Tf = RosMessageTypes.Tf2.TFMessageMsg;
using RosPosVector3 = Unity.Robotics.ROSTCPConnector.ROSGeometry.Vector3<Unity.Robotics.ROSTCPConnector.ROSGeometry.FLU>;
using RosQuaternion = Unity.Robotics.ROSTCPConnector.ROSGeometry.Quaternion<Unity.Robotics.ROSTCPConnector.ROSGeometry.FLU>;

/// <summary>
/// TF message subscriber
/// </summary>
public class SubTF : MonoBehaviour
{
    [SerializeField] private string topicName = "/tf";

    [SerializeField] GameObject Robot;
    [SerializeField] GameObject odom;
    [SerializeField] private bool isDebug = false;


    private UrdfLink[] UrdfLinkChain;

    private int numRobotLinks = 0;

    private Transform[] robotLinkPositions;

    // Start is called before the first frame update
    void Start()
    {
        if (Robot == null)
            Robot = GameObject.FindGameObjectWithTag("robot");

        if (!isDebug)
            Debug.unityLogger.logEnabled = false;

        ROSConnection.GetOrCreateInstance().Subscribe<Tf>(topicName, TfUpdate);
        UrdfLinkChain = Robot.GetComponentsInChildren<UrdfLink>();

        numRobotLinks =UrdfLinkChain.Length;
        
        robotLinkPositions = new Transform[numRobotLinks];
        Debug.Log("UrdfLinkChain.Length: " + numRobotLinks);

        for (int i = 0; i < numRobotLinks; i++)
        {
            robotLinkPositions[i] = UrdfLinkChain[i].gameObject.transform;
            Debug.Log($"robotLinkPositions{i} : {robotLinkPositions[i].name}");
        }

        Debug.Log(robotLinkPositions[1].name + " " + robotLinkPositions[1].localPosition + " " + robotLinkPositions[1].rotation);
    }

    /// <summary>
    /// デバッグ時の文字列生成用関数
    /// </summary>
    string Vector3ToString(Vector3 vec, string titile = "")
    {
        return titile + " (" + vec[0] + ", " + vec[1] + ", " + vec[2] + ")";
    }

    /// <summary>
    /// デバッグ時の文字列生成用関数
    /// </summary>
    string RosPosVector3ToString(RosPosVector3 vec, string titile = "")
    {
        return titile + " (" + vec.x + ", " + vec.y + ", " + vec.z + ")";
    }

    /// <summary>
    /// ROSトピックを受け取った際に呼ばれるコールバック関数
    /// </summary>
    void TfUpdate(Tf tfMessage)
    {
        for (int i = 0; i < tfMessage.transforms.Length; i++)
        {
            Debug.Log(tfMessage.transforms[i].child_frame_id);
            switch (tfMessage.transforms[i].child_frame_id)
            {
                case "locobot/base_footprint":
                    robotLinkPositions[0].position = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[0].rotation = translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/base_link":
                    //robotLinkPositions[1].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    //robotLinkPositions[1].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/odom":
                    odom.transform.localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    odom.transform.localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/gripper_link":
                    robotLinkPositions[18].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[18].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/wrist_link":
                    robotLinkPositions[17].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[17].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/shoulder_link":
                    robotLinkPositions[14].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[14].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/tilt_link":
                    robotLinkPositions[6].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[6].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/upper_arm_link":
                    robotLinkPositions[15].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[15].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/right_wheel":
                    //robotLinkPositions[46].position = translatePos(tfMessage.transforms[i].transform.translation);
                    //robotLinkPositions[46].rotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/right_finger_link":
                    robotLinkPositions[23].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[23].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/pan_link":
                    robotLinkPositions[5].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[5].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/left_wheel":
                    //robotLinkPositions[48].position = translatePos(tfMessage.transforms[i].transform.translation);
                    //robotLinkPositions[48].rotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/left_finger_link":
                    robotLinkPositions[24].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[24].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/gripper_prop_link":
                    robotLinkPositions[26].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[26].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
                case "locobot/forearm_link":
                    robotLinkPositions[16].localPosition = translatePos(tfMessage.transforms[i].transform.translation);
                    robotLinkPositions[16].localRotation= translateRot(tfMessage.transforms[i].transform.rotation);
                    break;
            }
        }
    }

    // 座標変換
    // 座標変換に関するのドキュメント
    // https://github.com/Unity-Technologies/ROS-TCP-Connector/blob/v0.5.0/ROSGeometry.md
    // 上記ドキュメントの実装箇所
    // https://github.com/Unity-Technologies/ROS-TCP-Connector/blob/v0.5.0/com.unity.robotics.ros-tcp-connector/Runtime/ROSGeometry/CoordinateSpaces.cs

    Vector3 translatePos(Vector3Msg posMsg)
    {
        // 座標変換はこちらでもOK (1)
        // Vector3 unityPos = tfMessage.transforms[i].transform.translation.From<FLU>();
        // 座標変換はこちらでもOK (2)
        // Vector3 unityPos = new Vector3((float)-tfMessage.transforms[i].transform.translation.y,
        //                             (float)tfMessage.transforms[i].transform.translation.z,
        //                             (float)tfMessage.transforms[i].transform.translation.x);

        RosPosVector3 rosPos = posMsg.As<FLU>();
        Vector3 unityPos = rosPos.toUnity;

        Debug.Log(RosPosVector3ToString(rosPos, "tf(ros)") + " --> " + Vector3ToString(unityPos, "tf(unity)"));

        return unityPos;
    }

    Quaternion translateRot(QuaternionMsg rotMsg)
    {
        // 座標変換はこちらでもOK (1)
        // Quaternion unityQuaternion = tfMessage.transforms[i].transform.rotation.From<FLU>();
        // 座標変換はこちらでもOK (2)
        // Quaternion unityQuaternion = new Quaternion((float)-tfMessage.transforms[i].transform.rotation.y,
        //                                 (float)tfMessage.transforms[i].transform.rotation.z,
        //                                 (float)tfMessage.transforms[i].transform.rotation.x,
        //                                 (float)-tfMessage.transforms[i].transform.rotation.w);

        RosQuaternion rosQuaternion = rotMsg.As<FLU>();
        Quaternion unityQuaternion = rosQuaternion.toUnity;

        Debug.Log($"rot: {unityQuaternion} ");

        return unityQuaternion;
    }
}
