using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.UI;
using TMPro;
using URV;


public class RosConnector : MonoBehaviour
{
    private ROSConnection ros;
    public PhotonConnection photonConnection;
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

    public UIManager uI;

    public bool isDebug = true;

    private void Awake()
    {
        if (!isDebug)
            Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        photonConnection = PhotonConnection.Disconnect;
        rosConnection = RosConnection.Disconnect;
        uI.punConnectButton.onClick.AddListener(() => punButton());
        uI.rosConnectButton.onClick.AddListener(() => rosButton());

    }

    //3秒後にPhotonに接続されているように挙動
    IEnumerator DummyPhotonConect()
    {
        uI.punConnection_Text.text = "Photon : Connecting";
        yield return new WaitForSeconds(1);
        OnPhotonConnect();
    }

    public void ConnectToPun()
    {
        if (photonConnection==PhotonConnection.Disconnect)
        {
            // PUNへの接続処理を行う
            try
            {
                StartCoroutine("DummyPhotonConect");   //Photon導入前にデバックで使用
                
            }
            catch (Exception e)
            {
                uI.punConnection_Text.text = "PUN : Failed to Connect:" + e.Message;
            }
        }
        else
        {
            uI.punConnection_Text.text = "Photon : Connected";
        }

    }

    public void OnPhotonConnect() // Photonが接続されたときに呼び出される関数
    {
        photonConnection = PhotonConnection.Connect;
        uI.punConnection_Text.text = "Photon : Connected";
        uI.punConnectButton.GetComponentInChildren<TMP_Text>().text = "Disconnect";
        uI.punConnectButton.interactable = true;
        uI.rosConnectButton.interactable = true;

    }

    public void DisconnectFromPun()
    {
        if (photonConnection==PhotonConnection.Connect)
        {
            try
            {
                //
                OnPhotonDisconnect();
            }
            catch (Exception e)
            {
                uI.punConnection_Text.text = "PUN : Failed to Disconnect:" + e.Message;
            }
        }
        else Debug.Log("PUN : Already disconnected");
    }

    public void OnPhotonDisconnect() // Photonが切断されたときに呼び出される関数
    {
        if (rosConnection==RosConnection.Connect)
        {
            DisconnectFromROS();
        }
        uI.punConnectButton.interactable = true;
        uI.rosConnectButton.interactable = false;
        photonConnection = PhotonConnection.Disconnect;
        uI.punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        uI.punConnection_Text.text = "Photon : Disconnected";
    }

    public void ConnectToROS() //ROSに接続するために呼び出す関数
    {
        if (photonConnection==PhotonConnection.Disconnect)
        {
            uI.punConnection_Text.text = "Photon : Not Connected";
            uI.rosConnection_Text.text = "Connect photon before connecting to ROS";
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
                    uI.rosConnection_Text.text = "ROS : Failed to Connect:" + e.Message;
                }
            }
            else
            {
                uI.punConnection_Text.text = "Photon : Connected";
                uI.rosConnection_Text.text = "ROS : Connected";
            }
        }
    }

    public void OnRosConnect() //ROS接続された時に呼び出される関数
                               //接続直後にPub/Subするメッセージはここで起動する
    {
        rosConnection = RosConnection.Connect;
        uI.rosConnection_Text.text = "ROS : Connected";
        uI.rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Disconnect";
        uI.punConnectButton.interactable = true;
        uI.rosConnectButton.interactable = true;

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
                uI.rosConnection_Text.text = "ROS : Disconnected";
                uI.rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
                uI.punConnectButton.interactable = true;
                uI.rosConnectButton.interactable = true;

                //Stop publish
                pubRosClock.OnRosDisconnected();
                //pubROSTransformTree.OnRosDisconnected();
                //pubTelecobotArmControl.OnRosDisconnected();
                //pubTelecobotBaseControl.OnRosDisconnected();
                pubUnityControl.OnRosDisconnected();

            }
            catch (Exception e)
            {
                uI.rosConnection_Text.text = "ROS : Failed to Disconnect:" + e.Message;
            }
        }
        else
        {
            uI.rosConnection_Text.text = "ROS : Already Disconnected";
        }
    }

    public void punButton()
    {
        uI.punConnectButton.interactable = false;
        uI.rosConnectButton.interactable = false;
        if (photonConnection == PhotonConnection.Disconnect) ConnectToPun();
        else DisconnectFromPun();
    }
    public void rosButton()
    {
        uI.punConnectButton.interactable = false;
        uI.rosConnectButton.interactable = false;
        if (rosConnection == RosConnection.Disconnect) ConnectToROS();
        else DisconnectFromROS();
    }
}
