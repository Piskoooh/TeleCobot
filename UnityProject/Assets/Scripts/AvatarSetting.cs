using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class AvatarSetting : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Role userRole;
    GameObject networkManager;
    PhotonManager PhotonManager => networkManager.GetComponent<PhotonManager>();
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// プロパティが更新された際に呼ばれるコールバック
    /// </summary>
    /// <param name="targetPlayer"></param>更新されたプレイヤー
    /// <param name="changedProps"></param>更新されたプロパティ
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //このオブジェクトの所有者を取得
        var avatarOwner = PhotonView.Get(this).Owner;
        //所有者のロールを取得し、ロールを設定する。
        int role = avatarOwner.GetRole();
        userRole = (Role)role;
        Debug.Log("AvatarSetting: " + avatarOwner.NickName + " is " + userRole);
        //オブジェクトとロールをディクショナリに保存する
        var success=PhotonManager.RoleDictionary.TryAdd(gameObject, userRole);
        if(!success)
        {
            //すでに存在する場合は上書きする
            PhotonManager.RoleDictionary[gameObject] = userRole;
        }
        //ロボットロールの場合はロボットリストに追加する
        if(userRole == Role.Robot)
        {
            PhotonManager.RobotList.Add(gameObject);
        }
    }
}
