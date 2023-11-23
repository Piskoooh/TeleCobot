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
    Role avatarRole;
    [SerializeField]
    Camera Camera;
    GameObject networkManager;
    PhotonManager PhotonManager;
    // Start is called before the first frame update
    void Awake()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        PhotonManager = networkManager.GetComponent<PhotonManager>();
    }

    private void Start()
    {
        if(Camera!=null)
            if (photonView.IsMine)
            {
                Camera.enabled = true;
            }
            else
            {
                Camera.enabled = false;
            }
    }
    // Update is called once per frame
    void Update()
    {
        if (Camera != null)
        {
            if (PhotonManager.focusRobot!=null)
            {
                var cameratarget = Camera.GetComponent<CameraFollow>().target;
                if (!cameratarget)
                    Camera.GetComponent<CameraFollow>().target = PhotonManager.focusRobot.transform;
            }
        }
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
        //所有者のロールプロパティを取得し、アバターのロールを設定
        int role = avatarOwner.GetRole();
        avatarRole = (Role)role;
        PhotonManager.AddToList(gameObject, avatarRole, PhotonView.Get(this).ViewID);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //ロールを削除
        var avatarOwner = PhotonView.Get(this).Owner;
        if (avatarOwner == otherPlayer)
        {
            PhotonManager.RemoveFromList(gameObject, avatarRole, PhotonView.Get(this).ViewID);
        }
    }
}
