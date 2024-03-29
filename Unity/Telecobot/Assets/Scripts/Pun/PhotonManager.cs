﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//https://zenn.dev/o8que/books/bdcb9af27bdd7d/viewer/c04ad5 を参考に作成。
//Photonの接続状態を管理するクラス
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public SceneMaster sceneMaster;
    public PhotonConnection photonConnection;

    //[HideInInspector]
    public GameObject MyAvatar;
    public List<int> robotList = new List<int>();//RobotのPhotonViewIDのリスト
    public SortedDictionary<int,int> RobotDictionary = new SortedDictionary<int, int>();// Key:PhotonViewID,Value:RosConnection
    public SortedDictionary<int,int> RoleDictionary = new SortedDictionary<int, int>();// Key:PhotonViewID,Value:Role

    public GameObject focusRobot;
    private int focusRobotID;

    // Start is called before the first frame update
    void Start()
    {
        photonConnection = PhotonConnection.Disconnect;
        sceneMaster.uIMng.punConnectButton.onClick.AddListener(() => PunButton());

        // プレイヤー自身の名前を"Player"に設定する
        PhotonNetwork.NickName = sceneMaster.userSettings.UserName.text;

        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }
    void Update()
    {
        //Memo:複数ロボットの際にフォーカスロボットの選択権限はOperetorに付与する。
        if(RobotDictionary.Count>0)
        {
            if (robotList.Count==1)
                if (focusRobot != PhotonView.Find(robotList[0]).gameObject)
                    PhotonNetwork.CurrentRoom.SetFocusRobot(robotList[0]);
            else
            {
                //ここにロボットが複数あったときの処理を加える。
            }
        }
        else
        {
            if (focusRobot != null)
            {
                focusRobot = null;
                PhotonNetwork.CurrentRoom.SetFocusRobot(0);
            }
        }
    }

    private void UpdateRobotFocus()
    {
        sceneMaster.rosConnector.GetRobot(focusRobot);
        if (MyAvatar.GetComponent<CameraFollowPun>().isActiveAndEnabled)
            MyAvatar.GetComponent<CameraFollowPun>().target = sceneMaster.rosConnector.base_link.transform;
    }

    private void LateUpdate()
    {
        if (PhotonNetwork.NetworkClientState==ClientState.Joined)
        {
            //カスタムプロパティの変化したものを更新
            PhotonNetwork.LocalPlayer.SendProperties();
            PhotonNetwork.CurrentRoom.SendProperties();
        }
    }

    private void OnApplicationQuit()
    {
        if (photonConnection == PhotonConnection.Connect)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            photonConnection = PhotonConnection.Disconnect;
            PhotonNetwork.Disconnect();
        }
    }

    public void PunButton()
    {
        sceneMaster.uIMng.punConnectButton.interactable = false;
        sceneMaster.uIMng.rosConnectButton.interactable = false;
        if (photonConnection == PhotonConnection.Disconnect) PhotonNetwork.ConnectUsingSettings();
        else PhotonNetwork.Disconnect();
    }

    public void AddToRoleDic(int viewID, int avatarRole)
    {
#if UNITY_2021_3_OR_NEWER
        //オブジェクトとロールをディクショナリに保存
        if (RoleDictionary.TryAdd(viewID, avatarRole))
        {
            Debug.Log("RoleDic:  Add: " + viewID);
        }
        else
        {
            //すでに存在する場合は上書き
            RoleDictionary[viewID] = avatarRole;
            Debug.Log("RoleDic:  Update: " + viewID);
        }
#else
        // オブジェクトとロールをディクショナリに保存

        // キーが存在しない場合は新しいキーとして追加
        if (!RoleDictionary.ContainsKey(viewID))
        {
            RoleDictionary.Add(viewID, avatarRole);
            Debug.Log("RoleDic: Add: " + viewID);
        }
        else
        {
            // すでに存在する場合は上書き
            RoleDictionary[viewID] = avatarRole;
            Debug.Log("RoleDic: Update: " + viewID);
        }
#endif
        DebugList();
    }

    public void AddToRobotDic(int viewID,int rosConect)
    {
#if UNITY_2021_3_OR_NEWER
        // ロボットのIDとROS接続状態をディクショナリに保存
        if (RobotDictionary.TryAdd(viewID, rosConect))
        {
            Debug.Log($"RobotDic:  Add: {viewID} : {(RosConnection)rosConect}");
        }
        else
        {
            //すでに存在する場合は上書き
            RobotDictionary[viewID] = rosConect;
            Debug.Log($"RobotDic:  Update: {viewID} : {(RosConnection)rosConect}");
        }
#else
        // ロボットのIDとROS接続状態をディクショナリに保存

        // キーが存在しない場合は新しいキーとして追加
        if (!RobotDictionary.ContainsKey(viewID))
        {
            RobotDictionary.Add(viewID, rosConect);
            Debug.Log($"RobotDic: Add: {viewID} : {(RosConnection)rosConect}");
        }
        else
        {
            // すでに存在する場合は上書き
            RobotDictionary[viewID] = rosConect;
            Debug.Log($"RobotDic: Update: {viewID} : {(RosConnection)rosConect}");
        }
#endif
        DebugList();
    }

    public void RemoveFromRoleDic(int viewID)
    {
        //オブジェクトとロールをディクショナリから削除
        if (RoleDictionary.Remove(viewID))
        {
            Debug.Log("RoleDic:  Remove: " + viewID);
        }
        else
        {
            Debug.Log("RoleDic:  Failed to remove: " + viewID);
        }
        DebugList();
    }

    public void RemoveFromRobotDic(int viewID)
    {
        if (RobotDictionary.Remove(viewID))
        {
            Debug.Log("RobotDic:  Remove: " + viewID);
        }
        else
        {
            Debug.Log("RobotDic:  Failed to remove: " + viewID);
        }
        DebugList();
    }

    public void DebugList()
    {
        foreach (var pair in RoleDictionary)
        {
            Debug.Log("RoleDic:  Key" + pair.Key + "Value" + (Role)pair.Value);
        }
        foreach (var pair1 in RobotDictionary)
        {
            Debug.Log("RobotDic:  Key" + pair1.Key + "Value" + (RosConnection)pair1.Value);
        }
    }

    #region PunCallbacks
    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.SetRole((int)sceneMaster.userSettings.role);
        if (sceneMaster.userSettings.userType == UserType.Robot)
        {
            PhotonNetwork.Instantiate("LocobotPun", Vector3.zero, Quaternion.identity);
            MyAvatar = PhotonNetwork.Instantiate("CameraPun", Vector3.up, Quaternion.identity);
        }
        else if (sceneMaster.userSettings.userType == UserType.Remote_VR)
        {
            if (sceneMaster.userSettings.role == Role.Operator)
                MyAvatar=PhotonNetwork.Instantiate("VROperatorCameraPun",Vector3.zero,Quaternion.identity);
            else
                MyAvatar = PhotonNetwork.Instantiate("VRCameraPun", Vector3.zero, Quaternion.identity);
        }
        else if (sceneMaster.userSettings.userType == UserType.Remote_nonVR)
        {
            if (sceneMaster.userSettings.role == Role.Operator)
                MyAvatar = PhotonNetwork.Instantiate("CameraOperatorPun", new Vector3(0,1,25), Quaternion.Euler(15,0,0));
            else
                MyAvatar = PhotonNetwork.Instantiate("CameraPun", new Vector3(0,1,25), Quaternion.Euler(15,0,0));
        }
        else if (sceneMaster.userSettings.userType == UserType.Local_AR)
        {
            Debug.LogError("Local_AR is not supported yet.");
            PhotonNetwork.Disconnect();
            return;
            // MyAvatar = PhotonNetwork.Instantiate("ARCameraPun", Vector3.up, Quaternion.identity);
        }
        else Debug.LogError("Unkown User Type. Cannot instatiate avatar.");
        //プレイヤーロールをカスタムプロパティに登録
        photonConnection = PhotonConnection.Connect;
        sceneMaster.uIMng.punConnection_Text.text = "Photon : Connected";
        sceneMaster.uIMng.punConnectButton.GetComponentInChildren<TMP_Text>().text = "Disconnect";
        sceneMaster.uIMng.punConnectButton.interactable = true;
        sceneMaster.uIMng.rosConnectButton.interactable = true;
    }

    public override void OnConnected()
    {
        base.OnConnected();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (sceneMaster.userSettings.role==Role.Robot&& sceneMaster.rosConnector.rosConnection == RosConnection.Connect)
        {
            sceneMaster.rosConnector.DisconnectFromROS();
        }
        sceneMaster.uIMng.punConnectButton.interactable = true;
        sceneMaster.uIMng.rosConnectButton.interactable = false;
        photonConnection = PhotonConnection.Disconnect;
        sceneMaster.uIMng.punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        sceneMaster.uIMng.punConnection_Text.text = "Photon : Disconnected";

        StartCoroutine(sceneMaster.userSettings.UnloadSceneAsync(sceneMaster.userSettings.CurrentSceneBuildIndex));
#if !UNITY_ANDROID
        if (sceneMaster.userSettings.userType == UserType.Remote_VR)
        {
            var manualXRControl = new ManualXRControl();
            manualXRControl.StopXR();
        }
#endif
        sceneMaster.userSettings.startCamera.gameObject.SetActive(true);
        foreach(Canvas canvas in sceneMaster.userSettings.startCanvas)
            canvas.gameObject.SetActive(true);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // カスタムプロパティが更新されたプレイヤーのプレイヤー名とIDをコンソールに出力する
        Debug.Log($"{targetPlayer.NickName}({targetPlayer.ActorNumber})");

        // 更新されたプレイヤーのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in changedProps)
        {
            if ((string)prop.Key == "R")
                Debug.Log($"Role: {(Role)prop.Value}");
            else if ((string)prop.Key == "RC")
                Debug.Log($"ROS_CONNECTION: {(RosConnection)prop.Value}");
            else
                Debug.Log($"{prop.Key}: {prop.Value}");
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // 更新されたルームのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in propertiesThatChanged)
        {
            if ((string)prop.Key == "CM")
                Debug.Log($"CONTROL_MODE: {(ControlMode)prop.Value}");
            else if ((string)prop.Key == "MC")
                Debug.Log($"MANUAL_COMMAND: {(ManualCommands)prop.Value}");
            else if ((string)prop.Key == "SAC")
                Debug.Log($"SEMI_AUTO_COMMMAND: {(SemiAutomaticCommands)prop.Value}");
            else if ((string)prop.Key == "FR")
                Debug.Log($"FOCUS_ROBOT: {prop.Value}");
            else
                Debug.Log($"{prop.Key}: {prop.Value}");
        }

        focusRobotID= PhotonNetwork.CurrentRoom.GetFocusRobot();
        if (focusRobotID != 0)
        {
            focusRobot = PhotonView.Find(focusRobotID).gameObject;
            UpdateRobotFocus();
        }
    }
    #endregion
}
