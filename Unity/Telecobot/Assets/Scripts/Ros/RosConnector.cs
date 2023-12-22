using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine.UI;
using TMPro;
using URV;
using Photon.Pun;
using Photon.Realtime;

public class RosConnector : MonoBehaviourPunCallbacks
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
    private GameObject currentRobot;

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
        if(sceneMaster.userSettings.role!=Role.Robot)
            sceneMaster.uIMng.rosConnectButton.interactable = false;
        else
            sceneMaster.uIMng.rosConnectButton.interactable = true;
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
            sceneMaster.uIMng.punConnection_Text.text = "Photon : オフライン";
            sceneMaster.uIMng.rosConnection_Text.text = "ROS接続前にPhotonに接続してください";
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
                    sceneMaster.uIMng.rosConnection_Text.text = "ROS : 接続失敗:" + e.Message;
                }
            }
            else
            {
                sceneMaster.uIMng.punConnection_Text.text = "Photon : オンライン";
                sceneMaster.uIMng.rosConnection_Text.text = "ROS : オンライン";
            }
        }
    }

    private void OnRosConnect() //ROS接続された時に呼び出される関数
                               //接続直後にPub/Subするメッセージはここで起動する
    {
        rosConnection = RosConnection.Connect;
        sceneMaster.uIMng.rosConnection_Text.text = "ROS : オンライン";
        sceneMaster.uIMng.rosConnectButton.GetComponentInChildren<TMP_Text>().text = "切断する";
        sceneMaster.uIMng.punConnectButton.interactable = true;
        sceneMaster.uIMng.rosConnectButton.interactable = true;
        PhotonNetwork.LocalPlayer.SetRosConnection((int)rosConnection);

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
                OnRosDisconnect();

            }
            catch (Exception e)
            {
                sceneMaster.uIMng.rosConnection_Text.text = "ROS : 切断失敗:" + e.Message;
            }
        }
        else
            sceneMaster.uIMng.rosConnection_Text.text = "ROS : 切断";
    }

    private void OnRosDisconnect()
    {
        rosConnection = RosConnection.Disconnect;
        sceneMaster.uIMng.rosConnection_Text.text = "ROS : オフライン";
        sceneMaster.uIMng.rosConnectButton.GetComponentInChildren<TMP_Text>().text = "接続する";
        sceneMaster.uIMng.punConnectButton.interactable = true;
        sceneMaster.uIMng.rosConnectButton.interactable = true;
        PhotonNetwork.LocalPlayer.SetRosConnection((int)rosConnection);

        //Stop publish
        subTF.OnRosDisconnected();
        pubRosClock.OnRosDisconnected();
        //pubROSTransformTree.OnRosDisconnected();
        //pubTelecobotArmControl.OnRosDisconnected();
        //pubTelecobotBaseControl.OnRosDisconnected();
        pubUnityControl.OnRosDisconnected();
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
        PhotonNetwork.LocalPlayer.SetRosConnection((int)rosConnection);
    }
}
