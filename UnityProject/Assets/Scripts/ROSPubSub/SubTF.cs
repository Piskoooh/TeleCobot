using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

using RosMessageTypes.Geometry;
using Tf = RosMessageTypes.Tf2.TFMessageMsg;
using RosPosVector3 = Unity.Robotics.ROSTCPConnector.ROSGeometry.Vector3<Unity.Robotics.ROSTCPConnector.ROSGeometry.FLU>;
using RosQuaternion = Unity.Robotics.ROSTCPConnector.ROSGeometry.Quaternion<Unity.Robotics.ROSTCPConnector.ROSGeometry.FLU>;

/// <summary>
/// TF message subscriber
/// </summary>
public class SubTF : MonoBehaviour
{
    [SerializeField] private string topicName = "tf";

    [SerializeField] GameObject Robot;

    [SerializeField] private bool isDebug = false;

    private int numRobotLinks = 0;

    private Transform[] robotLinkPositions;

    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<Tf>(topicName, TfUpdate);

        robotLinkPositions = new Transform[Robot.transform.childCount];
        Debug.Log("Robot.transform.childCount: " + Robot.transform.childCount);
        for (int i = 0; i < Robot.transform.childCount; i++)
        {
            robotLinkPositions[i] = Robot.transform.GetChild(i);
            Debug.Log(robotLinkPositions[i].name);
        }


        if (isDebug)
        {
            Debug.Log(robotLinkPositions[1].position + " " + robotLinkPositions[1].rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
            if (tfMessage.transforms[i].child_frame_id == "base_footprint")
            {

                Vector3Msg rosPosMsg = tfMessage.transforms[i].transform.translation;
                QuaternionMsg rosQuaternionMsg = tfMessage.transforms[i].transform.rotation;

                // 座標変換
                // 座標変換に関するのドキュメント
                // https://github.com/Unity-Technologies/ROS-TCP-Connector/blob/v0.5.0/ROSGeometry.md
                // 上記ドキュメントの実装箇所
                // https://github.com/Unity-Technologies/ROS-TCP-Connector/blob/v0.5.0/com.unity.robotics.ros-tcp-connector/Runtime/ROSGeometry/CoordinateSpaces.cs
                RosPosVector3 rosPos = rosPosMsg.As<FLU>();
                Vector3 unityPos = rosPos.toUnity;
                if (isDebug)
                {
                    Debug.Log(RosPosVector3ToString(rosPos, "tf(ros)") + " --> " + Vector3ToString(unityPos, "tf(unity)"));
                }

                RosQuaternion rosQuaternion = rosQuaternionMsg.As<FLU>();
                Quaternion unityQuaternion = rosQuaternion.toUnity;

                // 座標変換はこちらでもOK (1)
                // Vector3 unityPos = tfMessage.transforms[i].transform.translation.From<FLU>();
                // Quaternion unityQuaternion = tfMessage.transforms[i].transform.rotation.From<FLU>();

                // 座標変換はこちらでもOK (2)
                // Vector3 unityPos = new Vector3((float)-tfMessage.transforms[i].transform.translation.y,
                //                             (float)tfMessage.transforms[i].transform.translation.z,
                //                             (float)tfMessage.transforms[i].transform.translation.x);
                // Quaternion unityQuaternion = new Quaternion((float)-tfMessage.transforms[i].transform.rotation.y,
                //                                 (float)tfMessage.transforms[i].transform.rotation.z,
                //                                 (float)tfMessage.transforms[i].transform.rotation.x,
                //                                 (float)-tfMessage.transforms[i].transform.rotation.w);

                robotLinkPositions[0].position = unityPos;
                robotLinkPositions[0].rotation = unityQuaternion;
                if (isDebug)
                {
                    Debug.Log(Vector3ToString(unityPos, "pos"));
                }
            }
        }
    }
}
