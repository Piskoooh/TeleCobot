using System;
using System.Collections;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.UI;
using TMPro;

public class RosConnector : MonoBehaviour
{
    private ROSConnection ros;
    private bool punConnected = false;
    private bool rosConnected = false;

    public TMP_Text punConnectionStatusText;
    public TMP_Text rosConnectionStatusText;

    public URV.VisualizationTopicsTab visualizationTopicsTab;
    public SubJointState subJointState;
    public SubMoveitResponce subMoveitResponce;
    public PubRosClock pubRosClock;
    public PubROSTransformTree pubROSTransformTree;
    public PubTargetEndEffector pubTargetEndEffector;

    private void Start()
    {
        punConnectionStatusText.text = "Photon : Not Connected";
        rosConnectionStatusText.text = "Ros : Not Connected";

        StartCoroutine("DummyPhotonConect");   //Photon導入前にデバックで使用

    }

    //3秒後にPhotonに接続されているように挙動
    IEnumerator DummyPhotonConect()
    {
        punConnectionStatusText.text = "Photon : Connecting";
        yield return new WaitForSeconds(3);
        OnPhotonConnect();
    }

    public void OnPhotonConnect() // Photonが接続されたときに呼び出される関数
    {
        punConnected = true;
        punConnectionStatusText.text = "Photon : Connected";
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
        visualizationTopicsTab.OnRosConnect();
        subJointState.OnRosConnect();

        pubRosClock.OnRosConnect();
        pubROSTransformTree.OnRosConnect();
        pubTargetEndEffector.OnRosConnect();
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
            }
            catch(Exception e)
            {
                rosConnectionStatusText.text = "ROS : Failed to Disconnect:" + e.Message;
            }
        }
        else
        {
            rosConnectionStatusText.text = "ROS : Already Disconnected";
        }
    }

    public void OnPhotonDisconnect() // Photonが切断されたときに呼び出される関数
    {
        if (rosConnected)
        {
            ros.Disconnect();
            rosConnected = false;
        }
        punConnected = false;
        punConnectionStatusText.text = "Photon : Disconnected";
        rosConnectionStatusText.text = "ROS : Disconnected";
    }

    // 他のメソッドやGUIイベントハンドラなど
}
