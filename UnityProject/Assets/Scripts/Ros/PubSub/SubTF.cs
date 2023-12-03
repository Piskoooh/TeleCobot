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
    public RosConnector rosConnector;

    // Start is called before the first frame update
    public void OnRosConnect()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<Tf>(topicName, TfUpdate);
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
            //Debug.Log(tfMessage.transforms[i].child_frame_id);
            switch (tfMessage.transforms[i].child_frame_id)
            {
                case "locobot/odom":
                    rosConnector.robotLinkPositions[0].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[0].rotation = translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/base_link":
                    rosConnector.robotLinkPositions[1].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[1].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/gripper_link":
                    rosConnector.robotLinkPositions[18].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[18].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/wrist_link":
                    rosConnector.robotLinkPositions[17].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[17].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/shoulder_link":
                    rosConnector.robotLinkPositions[14].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[14].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/tilt_link":
                    rosConnector.robotLinkPositions[6].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[6].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/upper_arm_link":
                    rosConnector.robotLinkPositions[15].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[15].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/right_wheel":
                    rosConnector.robotLinkPositions[46].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[46].rotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/right_finger_link":
                    rosConnector.robotLinkPositions[23].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[23].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/pan_link":
                    rosConnector.robotLinkPositions[5].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[5].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/left_wheel":
                    rosConnector.robotLinkPositions[48].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[48].localRotation = translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/left_finger_link":
                    rosConnector.robotLinkPositions[24].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[24].localRotation= translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/gripper_prop_link":
                    rosConnector.robotLinkPositions[26].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[26].localRotation = translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
                case "locobot/forearm_link":
                    rosConnector.robotLinkPositions[16].localPosition = translatePos(tfMessage.transforms[i].transform.translation, tfMessage.transforms[i].child_frame_id);
                    rosConnector.robotLinkPositions[16].localRotation = translateRot(tfMessage.transforms[i].transform.rotation, tfMessage.transforms[i].child_frame_id);
                    break;
            }
        }
    }

    // 座標変換
    // 座標変換に関するのドキュメント
    // https://github.com/Unity-Technologies/ROS-TCP-Connector/blob/v0.5.0/ROSGeometry.md
    // 上記ドキュメントの実装箇所
    // https://github.com/Unity-Technologies/ROS-TCP-Connector/blob/v0.5.0/com.unity.robotics.ros-tcp-connector/Runtime/ROSGeometry/CoordinateSpaces.cs

    Vector3 translatePos(Vector3Msg posMsg,string frame_id)
    {
        // 座標変換はこちらでもOK (1)
        // Vector3 unityPos = tfMessage.transforms[i].transform.translation.From<FLU>();
        // 座標変換はこちらでもOK (2)
        // Vector3 unityPos = new Vector3((float)-tfMessage.transforms[i].transform.translation.y,
        //                             (float)tfMessage.transforms[i].transform.translation.z,
        //                             (float)tfMessage.transforms[i].transform.translation.x);

        RosPosVector3 rosPos = posMsg.As<FLU>();
        Vector3 unityPos = rosPos.toUnity;

        //Debug.Log(frame_id + " : " + RosPosVector3ToString(rosPos, "tf(ros)") + " --> " + Vector3ToString(unityPos, "tf(unity)"));

        return unityPos;
    }

    Quaternion translateRot(QuaternionMsg rotMsg,string frame_id)
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

        //Debug.Log(frame_id + " : " + "rot: "+unityQuaternion);

        return unityQuaternion;
    }
}
