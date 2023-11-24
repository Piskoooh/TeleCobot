using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

//https://zenn.dev/o8que/books/bdcb9af27bdd7d/viewer/c04ad5 を参考に作成。
//Photonの接続状態を管理するクラス
public class PhotonManager : MonoBehaviourPunCallbacks
{
    UserSettings userSettings;
    public PhotonConnection photonConnection;
    public RosConnector rosConnector;
    public UIManager uI;
    public InputManager inputManager;

    //[HideInInspector]
    public GameObject MyAvatar;
    public SortedDictionary<int,GameObject> RobotDictionary = new SortedDictionary<int, GameObject>();
    public Dictionary<GameObject,Role> RoleDictionary = new Dictionary<GameObject, Role>();

    public GameObject focusRobot;
    private int focusRobotID;

    private void Awake()
    {
        userSettings = GameObject.Find("UserSettings").GetComponent<UserSettings>();
    }

    // Start is called before the first frame update
    void Start()
    {
        photonConnection = PhotonConnection.Disconnect;
        uI.punConnectButton.onClick.AddListener(() => PunButton());

        // プレイヤー自身の名前を"Player"に設定する
        PhotonNetwork.NickName = userSettings.UserName.text;

        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }
    void Update()
    {
        if(RobotDictionary.Count>0)
        {
            if (focusRobot == null)
            {
                var firstRobot = RobotDictionary.First();
                focusRobotID = firstRobot.Key;
                focusRobot = firstRobot.Value;
                UpdateRobotFocus();
            }
            else if (focusRobot != null)
            {
                //ここにロボットが複数あったときの処理を加える。
            }
        }
        else
        {
            focusRobot = null;
            focusRobotID = 0;
        }
    }

    private void UpdateRobotFocus()
    {
        rosConnector.GetRobot(focusRobot);
        if (MyAvatar.GetComponent<CameraFollowPun>().isActiveAndEnabled)
            MyAvatar.GetComponent<CameraFollowPun>().target = rosConnector.base_link.transform;
    }

    private void LateUpdate()
    {
        //カスタムプロパティの変化したものを更新
        PhotonNetwork.LocalPlayer.SendProperties();
        PhotonNetwork.CurrentRoom.SendProperties();
    }

    private void OnApplicationQuit()
    {
        photonConnection = PhotonConnection.Disconnect;
    }
    public void PunButton()
    {
        uI.punConnectButton.interactable = false;
        uI.rosConnectButton.interactable = false;
        if (photonConnection == PhotonConnection.Disconnect) PhotonNetwork.ConnectUsingSettings();
        else PhotonNetwork.Disconnect();
    }

    public void AddToList(GameObject gameObject, Role avatarRole,int viewID)
    {
        //オブジェクトとロールをディクショナリに保存
        if (RoleDictionary.TryAdd(gameObject, avatarRole))
        {
            Debug.Log("Add: " + gameObject);
        }
        else
        {
            //すでに存在する場合は上書き
            RoleDictionary[gameObject] = avatarRole;
            Debug.Log("Update: " + gameObject);
        }
        //ロボットロールの場合はロボットリストに追加する
        if (avatarRole == Role.Robot&&gameObject.tag=="robot")
        {
            if(RobotDictionary.TryAdd(viewID, gameObject))
            {
                Debug.Log("Add: " + gameObject);
            }
            else
            {
                //すでに存在する場合は上書き
                RobotDictionary[viewID] = gameObject;
                Debug.Log("Update: " + gameObject);
            }
        }
        DebugList();
    }

    public void RemoveFromList(GameObject gameObject, Role avatarRole,int viewID)
    {
        //オブジェクトとロールをディクショナリから削除
        if (RoleDictionary.Remove(gameObject, out avatarRole))
        {
            Debug.Log("Remove: " + gameObject);
        }
        else
        {
            Debug.Log("Failed to remove: " + gameObject);
        }

        //ロボットロールの場合はロボットリストから削除する
        if (avatarRole == Role.Robot)
        {
            if(RobotDictionary.Remove(viewID))
            {
                Debug.Log("Remove: " + gameObject);
            }
            else
            {
                Debug.Log("Failed to remove: " + gameObject);
            }
        }
        DebugList();
    }

    public void DebugList()
    {
        foreach (var pair in RoleDictionary)
        {
            Debug.Log("Key" + pair.Key + "Value" + pair.Value);
        }
        foreach (var pair1 in RobotDictionary)
        {
            Debug.Log("Key" + pair1.Key + "Value" + pair1.Value);
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
        PhotonNetwork.LocalPlayer.SetRole((int)userSettings.role);
        if (userSettings.userType == UserType.Robot)
        {
            PhotonNetwork.Instantiate("LocobotPun", Vector3.zero, Quaternion.identity);
            MyAvatar = PhotonNetwork.Instantiate("CameraPun", Vector3.zero, Quaternion.identity);
        }
        else if (userSettings.userType == UserType.Remote_VR)
            MyAvatar = PhotonNetwork.Instantiate("VRCameraPun", Vector3.zero, Quaternion.identity);
        else if (userSettings.userType == UserType.Remote_nonVR)
            MyAvatar = PhotonNetwork.Instantiate("CameraPun", Vector3.zero, Quaternion.identity);
        else if (userSettings.userType == UserType.Local_AR)
            MyAvatar = PhotonNetwork.Instantiate("ARCameraPun", Vector3.zero, Quaternion.identity);
        else Debug.LogError("Unkown User Type. Cannot instatiate avatar.");
        //プレイヤーロールをカスタムプロパティに登録
        photonConnection = PhotonConnection.Connect;
        uI.punConnection_Text.text = "Photon : Connected";
        uI.punConnectButton.GetComponentInChildren<TMP_Text>().text = "Disconnect";
        uI.punConnectButton.interactable = true;
        uI.rosConnectButton.interactable = true;

        if (userSettings.role == Role.Operator)
        {
            PhotonNetwork.CurrentRoom.SetControlMode((int)inputManager.controlMode);
            PhotonNetwork.CurrentRoom.SetManualCmd((int)inputManager.manualCmd);
            PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)inputManager.semiAutoCmd);
        }
        else
        {
            inputManager.controlMode = (ControlMode)PhotonNetwork.CurrentRoom.GetControlMode();
            inputManager.manualCmd = (ManualCommands)PhotonNetwork.CurrentRoom.GetManualCmd();
            inputManager.semiAutoCmd = (SemiAutomaticCommands)PhotonNetwork.CurrentRoom.GetSemiAutoCmd();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        if (rosConnector.rosConnection == RosConnection.Connect)
        {
            rosConnector.DisconnectFromROS();
        }
        uI.punConnectButton.interactable = true;
        uI.rosConnectButton.interactable = false;
        photonConnection = PhotonConnection.Disconnect;
        uI.punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        uI.punConnection_Text.text = "Photon : Disconnected";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // カスタムプロパティが更新されたプレイヤーのプレイヤー名とIDをコンソールに出力する
        Debug.Log($"{targetPlayer.NickName}({targetPlayer.ActorNumber})");

        // 更新されたプレイヤーのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in changedProps)
        {
            Debug.Log($"{prop.Key}: {prop.Value}");
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // 更新されたルームのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in propertiesThatChanged)
        {
            Debug.Log($"{prop.Key}: {prop.Value}");
        }
        inputManager.controlMode = (ControlMode)PhotonNetwork.CurrentRoom.GetControlMode();
        inputManager.manualCmd = (ManualCommands)PhotonNetwork.CurrentRoom.GetManualCmd();
        inputManager.semiAutoCmd = (SemiAutomaticCommands)PhotonNetwork.CurrentRoom.GetSemiAutoCmd();
    }
    #endregion
}
