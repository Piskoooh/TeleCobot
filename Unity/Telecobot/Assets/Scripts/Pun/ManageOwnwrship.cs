using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// リクエストを処理するためのクラス
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class ManageOwnership : MonoBehaviour, IPunOwnershipCallbacks, IPunObservable
{
    PhotonView photonView;

    /// <summary>
    /// 権限移譲可否
    /// </summary>
    private bool changeableOwner = true;

    void Start()
    {
        // PhotonViewの存在確認
        photonView = this.GetComponent<PhotonView>();
        if (photonView == null)
        {
            // 存在しない場合は無効化
            this.enabled = false;
        }
    }
    /// <summary>
    /// スクリプト有効化時の処理
    /// </summary>
    void OnEnable()
    {
        // Callbackに追加
        PhotonNetwork.AddCallbackTarget(this);
    }

    /// <summary>
    /// スクリプト無効化時の処理
    /// </summary>
    void OnDisable()
    {
        // Callbackから削除
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// 権限移譲可否変更
    /// </summary>
    /// <param name="value"></param>
    public void ChangeOwnerShipState(bool value)
    {
        if (photonView.IsMine)
        {
            // 権限がある場合のみ変更する
            changeableOwner = value;
        }
    }

    /// <summary>
    /// リクエスト処理
    /// </summary>
    /// <param name="targetView"></param>
    /// <param name="requestingPlayer"></param>
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {

        if (changeableOwner)
        {
            // 権限移譲できる場合
            targetView.TransferOwnership(requestingPlayer);
            Debug.Log("権限取得要請");
        }
        else
        {
            Debug.Log("権限取得不可");
        }


    }

    /// <summary>
    /// リクエスト移譲時処理
    /// </summary>
    /// <param name="targetView"></param>
    /// <param name="requestingPlayer"></param>
    public void OnOwnershipTransfered(PhotonView targetView, Player requestingPlayer)
    {
        // 処理なし
    }

    /// <summary>
    /// 権限移譲失敗時処理
    /// </summary>
    /// <param name="targetView"></param>
    /// <param name="requestingPlayer"></param>
    public void OnOwnershipTransferFailed(PhotonView targetView, Player requestingPlayer)
    {
        // 処理なし
    }

    /// <summary>
    /// 権限移譲可否状態の共有
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 権限移譲可否状態を送信
            stream.SendNext(changeableOwner);
        }
        else
        {
            // 権限移譲可否状態を受信
            changeableOwner = (bool)stream.ReceiveNext();
        }

    }
}
