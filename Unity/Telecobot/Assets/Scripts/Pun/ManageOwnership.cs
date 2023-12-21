using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class ManageOwnership : MonoBehaviour, IPunOwnershipCallbacks, IPunObservable
{
    PhotonView photonView;

    private bool changeableOwner = true;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            enabled = false;
        }
    }
    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void ChangeOwnerShipState(bool value)
    {
        if (photonView.IsMine)
        {
            changeableOwner = value;
        }
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {

        if (changeableOwner)
        {
            targetView.TransferOwnership(requestingPlayer);
        }
        else
        {
        }

    }

    public void OnOwnershipTransfered(PhotonView targetView, Player requestingPlayer)
    {
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player requestingPlayer)
    {
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(changeableOwner);
        }
        else
        {
            changeableOwner = (bool)stream.ReceiveNext();
        }
    }
}
