using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine.UI;
using TMPro;
using URV;


public class RosConnector : MonoBehaviour
{
    public SceneMaster sceneMaster;
    private ROSConnection ros;
    public RosConnection rosConnection;

    public VisualizationTopicsTab visualizationTopicsTab;
    public SubTF subTF;
    //public SubJointState subJointState;
    //public SubCmdVel subCmdVel;
    public PubRosClock pubRosClock;
    //public PubROSTransformTree pubROSTransformTree;
    //public PubTelecobotArmControl pubTelecobotArmControl;
    //public PubTelecobotBaseControl pubTelecobotBaseControl;
    public PubUnityControl pubUnityControl;
    [HideInInspector]
    public GameObject currentRobot;

    private UrdfLink[] UrdfLinkChain;
    private int numRobotLinks = 0;
    [HideInInspector]
    public Transform[] robotLinkPositions;

    public GameObject endEffector;
    public GameObject base_link;
    public GameObject arm_base_link;

    public bool isDebug = true;

    private void Awake()
    {
        if (!isDebug)
            Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        rosConnection = RosConnection.Disconnect;
        sceneMaster.uIMng.rosConnectButton.onClick.AddListener(() => rosButton());
    }

    public void GetRobot(GameObject robot)
    {
        if(currentRobot != robot)
        {
            UrdfLinkChain = robot.GetComponentsInChildren<UrdfLink>();
            numRobotLinks = UrdfLinkChain.Length;
            robotLinkPositions = new Transform[numRobotLinks];

            for (int i = 0; i < numRobotLinks; i++)
            {
                robotLinkPositions[i] = UrdfLinkChain[i].gameObject.transform;
                switch (UrdfLinkChain[i].gameObject.name)
                {
                    case "/base_link":
                        base_link = UrdfLinkChain[i].gameObject;
                        break;
                    case "/arm_base_link":
                        arm_base_link = UrdfLinkChain[i].gameObject;
                        break;
                    case "/ee_gripper_link":
                        endEffector = UrdfLinkChain[i].gameObject;
                        break;
                }
            }
            currentRobot = robot;
        }
    }

    public void ConnectToROS() //ROSに接続するために呼び出す関数
    {
        if (sceneMaster.photonMng.photonConnection==PhotonConnection.Disconnect)
        {
            sceneMaster.uIMng.punConnection_Text.text = "Photon : Not Connected";
            sceneMaster.uIMng.rosConnection_Text.text = "Connect photon before connecting to ROS";
        }
        else
        {
            if (rosConnection==RosConnection.Disconnect)
            {
                ros = ROSConnection.GetOrCreateInstance();
                // ROSへの接続処理を行う
                try
                {
                    ros.Connect();
                    OnRosConnect();
                }
                catch (Exception e)
                {
                    sceneMaster.uIMng.rosConnection_Text.text = "ROS : Failed to Connect:" + e.Message;
                }
            }
            else
            {
                sceneMaster.uIMng.punConnection_Text.text = "Photon : Connected";
                sceneMaster.uIMng.rosConnection_Text.text = "ROS : Connected";
            }
        }
    }

    public void OnRosConnect() //ROS接続された時に呼び出される関数
                               //接続直後にPub/Subするメッセージはここで起動する
    {
        rosConnection = RosConnection.Connect;
        sceneMaster.uIMng.rosConnection_Text.text = "ROS : Connected";
        sceneMaster.uIMng.rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Disconnect";
        sceneMaster.uIMng.punConnectButton.interactable = true;
        sceneMaster.uIMng.rosConnectButton.interactable = true;

        //visualization
        visualizationTopicsTab.OnRosConnect();

        //subscribe
        subTF.OnRosConnect();
        //subJointState.OnRosConnect();
        //subCmdVel.OnRosConnect();

        //publish
        pubRosClock.OnRosConnect();
        //pubROSTransformTree.OnRosConnect();
        //pubTelecobotArmControl.OnRosConnect();
        //pubTelecobotBaseControl.OnRosConnect();
        pubUnityControl.OnRosConnect();
    }

    public void DisconnectFromROS()　//ROSから切断するために呼び出す関数
    {
        if (rosConnection==RosConnection.Connect)
        {
            try
            {
                ros.Disconnect();
                rosConnection = RosConnection.Disconnect;
                sceneMaster.uIMng.rosConnection_Text.text = "ROS : Disconnected";
                sceneMaster.uIMng.rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
                sceneMaster.uIMng.punConnectButton.interactable = true;
                sceneMaster.uIMng.rosConnectButton.interactable = true;

                //Stop publish
                pubRosClock.OnRosDisconnected();
                //pubROSTransformTree.OnRosDisconnected();
                //pubTelecobotArmControl.OnRosDisconnected();
                //pubTelecobotBaseControl.OnRosDisconnected();
                pubUnityControl.OnRosDisconnected();

            }
            catch (Exception e)
            {
                sceneMaster.uIMng.rosConnection_Text.text = "ROS : Failed to Disconnect:" + e.Message;
            }
        }
        else
            sceneMaster.uIMng.rosConnection_Text.text = "ROS : Already Disconnected";
    }

    public void rosButton()
    {
        sceneMaster.uIMng.punConnectButton.interactable = false;
        sceneMaster.uIMng.rosConnectButton.interactable = false;
        if (rosConnection == RosConnection.Disconnect) ConnectToROS();
        else DisconnectFromROS();
    }

    private void OnApplicationQuit()
    {
        rosConnection = RosConnection.Disconnect;
    }
}
