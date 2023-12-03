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


public class PhotonManager : MonoBehaviourPunCallbacks
{
    UserSettings userSettings;
    public PhotonConnection photonConnection;
    public RosConnector rosConnector;
    public UIManager uI;

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
            }
        }
        else
        {
            focusRobot = null;
            focusRobotID = 0;
        }
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
        if (avatarRole == Role.Robot)
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
        if (userSettings.userType == UserType.Robot)
        {
            MyAvatar = PhotonNetwork.Instantiate("LocobotPun", Vector3.zero, Quaternion.identity);
            rosConnector.GetRobot();
        }
        else if (userSettings.userType == UserType.Remote_VR)
            MyAvatar = PhotonNetwork.Instantiate("VRCameraPun", Vector3.zero, Quaternion.identity);
        else if (userSettings.userType == UserType.Remote_nonVR)
            MyAvatar = PhotonNetwork.Instantiate("CameraPun", Vector3.zero, Quaternion.identity);
        else if (userSettings.userType == UserType.Local_AR)
            MyAvatar = PhotonNetwork.Instantiate("ARCameraPun", Vector3.zero, Quaternion.identity);
        else Debug.LogError("Unkown User Type. Cannot instatiate avatar.");
        //プレイヤーロールをカスタムプロパティに登録
        PhotonNetwork.LocalPlayer.SetRole((int)userSettings.role);
        photonConnection = PhotonConnection.Connect;
        uI.punConnection_Text.text = "Photon : Connected";
        uI.punConnectButton.GetComponentInChildren<TMP_Text>().text = "Disconnect";
        uI.punConnectButton.interactable = true;
        uI.rosConnectButton.interactable = true;
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
    #endregion
}
