using System;
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
    PhotonManager PhotonManager;
    // Start is called before the first frame update
    void Awake()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        PhotonManager = networkManager.GetComponent<PhotonManager>();
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
        //所有者のロールを取得し、ロールを設定
        int role = avatarOwner.GetRole();
        userRole = (Role)role;
        AddToList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //ロールを削除
        var avatarOwner = PhotonView.Get(this).Owner;
        if (avatarOwner == otherPlayer)
        {
            RemoveFromList();
        }

    }

    private void AddToList()
    {
        //オブジェクトとロールをディクショナリに保存
        if (PhotonManager.RoleDictionary.TryAdd(gameObject, userRole))
        {
            Debug.Log("Add: " + gameObject);
        }
        else
        {
            //すでに存在する場合は上書き
            PhotonManager.RoleDictionary[gameObject] = userRole;
            Debug.Log("Update: " + gameObject);
        }
        //ロボットロールの場合はロボットリストに追加する
        if (userRole == Role.Robot)
        {
            PhotonManager.RobotList.Add(gameObject);
        }
        PhotonManager.DebugList();
    }

    private void RemoveFromList()
    {
        //オブジェクトとロールをディクショナリから削除
        if (PhotonManager.RoleDictionary.Remove(gameObject, out userRole))
        {
            Debug.Log("Remove: " + gameObject);
        }
        else
        {
            Debug.Log("Failed to remove: " + gameObject);
        }

        //ロボットロールの場合はロボットリストから削除する
        if (userRole == Role.Robot)
        {
            PhotonManager.RobotList.Remove(gameObject);
        }
        PhotonManager.DebugList();
    }   
}
