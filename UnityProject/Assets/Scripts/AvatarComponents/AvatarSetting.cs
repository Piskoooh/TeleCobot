﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(CameraControllerPun))]
[RequireComponent(typeof(CameraFollowPun))]

public class AvatarSetting : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Role avatarRole;
    [SerializeField]
    Camera Camera;
    [HideInInspector]
    public SceneMaster sceneMaster;
    // Start is called before the first frame update
    void Awake()
    {
        sceneMaster = GameObject.FindGameObjectWithTag("SceneMaster").GetComponent<SceneMaster>();
    }

    private void SetAvatarRoll()
    {
        //このオブジェクトの所有者を取得
        var avatarOwner = PhotonView.Get(this).Owner;
        //所有者のロールプロパティを取得し、アバターのロールを設定
        int role = avatarOwner.GetRole();
        avatarRole = (Role)role;
    }

    private void SetAvatarCamera()
    {
        if (Camera != null)
            if (photonView.IsMine)
            {
                Camera.enabled = true;
                if (avatarRole == Role.Robot)
                {
                    var cf = Camera.GetComponent<CameraFollowPun>();
                    cf.enabled = true;
                }
                else
                    Camera.GetComponent<CameraControllerPun>().enabled = true;
            }
            else
                Camera.enabled = false;
    }

    /// <summary>
    /// プロパティが更新された際に呼ばれるコールバック
    /// </summary>
    /// <param name="targetPlayer"></param>更新されたプレイヤー
    /// <param name="changedProps"></param>更新されたプロパティ
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        SetAvatarRoll();
        sceneMaster.photonMng.AddToList(gameObject, avatarRole, PhotonView.Get(this).ViewID);
        SetAvatarCamera();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        //ロールを削除
        var avatarOwner = PhotonView.Get(this).Owner;
        if (avatarOwner == otherPlayer)
            sceneMaster.photonMng.RemoveFromList(gameObject, avatarRole, PhotonView.Get(this).ViewID);
    }
}
