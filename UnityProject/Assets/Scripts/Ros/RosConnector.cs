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
    private bool punConnected = false;
    private bool rosConnected = false;

    public TMP_Text punConnectionStatusText, rosConnectionStatusText;
    public Button punConnectButton, rosConnectButton;
    public CanvasGroup rosUIs;

    public VisualizationTopicsTab visualizationTopicsTab;
    public SubTF subTF;
    //public SubJointState subJointState;
    //public SubCmdVel subCmdVel;
    public PubRosClock pubRosClock;
    //public PubROSTransformTree pubROSTransformTree;
    public PubTelecobotArmControl pubTelecobotArmControl;
    public PubTelecobotBaseControl pubTelecobotBaseControl;

    public bool isDebug = true;

    private void Awake()
    {
        if (!isDebug)
            Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        punConnectionStatusText.text = "Photon : Not Connected";
        rosConnectionStatusText.text = "Ros : Not Connected";
        punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.interactable = false;
        rosUIs.interactable = false;
        punConnected = false;
        rosConnected = false;
        punConnectButton.onClick.AddListener(() => punButton());
        rosConnectButton.onClick.AddListener(() => rosButton());

    }

    //3秒後にPhotonに接続されているように挙動
    IEnumerator DummyPhotonConect()
    {
        punConnectionStatusText.text = "Photon : Connecting";
        yield return new WaitForSeconds(1);
        OnPhotonConnect();
    }

    public void ConnectToPun()
    {
        if (!punConnected)
        {
            // PUNへの接続処理を行う
            try
            {
                StartCoroutine("DummyPhotonConect");   //Photon導入前にデバックで使用
                
            }
            catch (Exception e)
            {
                punConnectionStatusText.text = "PUN : Failed to Connect:" + e.Message;
            }
        }
        else
        {
            punConnectionStatusText.text = "Photon : Connected";
        }

    }

    public void OnPhotonConnect() // Photonが接続されたときに呼び出される関数
    {
        punConnected = true;
        punConnectionStatusText.text = "Photon : Connected";
        punConnectButton.GetComponentInChildren<TMP_Text>().text = "Disconnect";
        punConnectButton.interactable = true;
        rosConnectButton.interactable = true;

    }

    public void DisconnectFromPun()
    {
        if (punConnected)
        {
            try
            {
                //
                OnPhotonDisconnect();
            }
            catch (Exception e)
            {
                punConnectionStatusText.text = "PUN : Failed to Disconnect:" + e.Message;
            }
        }
        else Debug.Log("PUN : Already disconnected");
    }

    public void OnPhotonDisconnect() // Photonが切断されたときに呼び出される関数
    {
        if (rosConnected)
        {
            DisconnectFromROS();
        }
        punConnectButton.interactable = true;
        rosConnectButton.interactable = false;
        punConnected = false;
        punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        punConnectionStatusText.text = "Photon : Disconnected";
    }

    public void ConnectToROS() //ROSに接続するために呼び出す関数
    {
        if (!punConnected)
        {
            punConnectionStatusText.text = "Photon : Not Connected";
            rosConnectionStatusText.text = "Connect photon before connecting to ROS";
        }
        else
        {
            if (!rosConnected)
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
                    rosConnectionStatusText.text = "ROS : Failed to Connect:" + e.Message;
                }
            }
            else
            {
                punConnectionStatusText.text = "Photon : Connected";
                rosConnectionStatusText.text = "ROS : Connected";
            }
        }
    }

    public void OnRosConnect() //ROS接続された時に呼び出される関数
                               //接続直後にPub/Subするメッセージはここで起動する
    {
        rosConnected = true;
        rosConnectionStatusText.text = "ROS : Connected";
        rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Disconnect";
        punConnectButton.interactable = true;
        rosConnectButton.interactable = true;
        rosUIs.interactable = true;

        //visualization
        visualizationTopicsTab.OnRosConnect();

        //subscribe
        subTF.OnRosConnect();
        //subJointState.OnRosConnect();
        //subCmdVel.OnRosConnect();

        //publish
        pubRosClock.OnRosConnect();
        //pubROSTransformTree.OnRosConnect();
        pubTelecobotArmControl.OnRosConnect();
        //pubTelecobotBaseControl.OnRosConnect();

    }

    public void DisconnectFromROS()　//ROSから切断するために呼び出す関数
    {
        if (rosConnected)
        {
            try
            {
                ros.Disconnect();
                rosConnected = false;
                rosConnectionStatusText.text = "ROS : Disconnected";
                rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
                rosUIs.interactable = false;
                punConnectButton.interactable = true;
                rosConnectButton.interactable = true;

                //Stop publish
                pubRosClock.OnRosDisconnected();
                //pubROSTransformTree.OnRosDisconnected();
                pubTelecobotArmControl.OnRosDisconnected();
                pubTelecobotBaseControl.OnRosDisconnected();

            }
            catch (Exception e)
            {
                rosConnectionStatusText.text = "ROS : Failed to Disconnect:" + e.Message;
            }
        }
        else
        {
            rosConnectionStatusText.text = "ROS : Already Disconnected";
        }
    }

    public void punButton()
    {
        punConnectButton.interactable = false;
        rosConnectButton.interactable = false;
        if (punConnected == false) ConnectToPun();
        else DisconnectFromPun();
    }
    public void rosButton()
    {
        punConnectButton.interactable = false;
        rosConnectButton.interactable = false;
        if (rosConnected == false) ConnectToROS();
        else DisconnectFromROS();
    }
}
