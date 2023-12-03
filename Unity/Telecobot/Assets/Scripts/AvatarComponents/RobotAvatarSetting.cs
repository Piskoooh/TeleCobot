using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RobotAvatarSetting : AvatarSetting
{
    public RosConnection robotRosConnection;
    private void SetRobotRosConnection()
    {
        //このオブジェクトの所有者を取得
        var avatarOwner = PhotonView.Get(this).Owner;
        //所有者のロールプロパティを取得し、アバターのロールを設定
        int rrc = avatarOwner.GetRosConnection();
        robotRosConnection = (RosConnection)rrc;
    }

    public void CallAARVP(string vI)
    {
        photonView.RPC(nameof(AdjustArmRangeVisualizerPun), RpcTarget.AllViaServer,vI);
    }

    public void CallEeA(string eE)
    {
        photonView.RPC(nameof(EeAttacherPun), RpcTarget.AllViaServer, eE);
    }

    [PunRPC]
    private void AdjustArmRangeVisualizerPun(string vI, PhotonMessageInfo info)
    {
        if (!info.Sender.IsLocal)
        {
            sceneMaster.uIMng.visualIndicator= GameObject.FindGameObjectWithTag(vI);
        }
        sceneMaster.uIMng.visualIndicator.transform.parent = sceneMaster.rosConnector.arm_base_link.transform;
        sceneMaster.uIMng.visualIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
        sceneMaster.uIMng.visualIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    [PunRPC]
    private void EeAttacherPun(string eE, PhotonMessageInfo info)
    {
        if(!info.Sender.IsLocal)
            sceneMaster.uIMng.eeGripper = GameObject.FindGameObjectWithTag(eE);
    }

    /// <summary>
    /// プロパティが更新された際に呼ばれるコールバック
    /// </summary>
    /// <param name="targetPlayer"></param>更新されたプレイヤー
    /// <param name="changedProps"></param>更新されたプロパティ
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        SetRobotRosConnection();
        sceneMaster.photonMng.AddToRobotDic(PhotonView.Get(this).ViewID, (int)robotRosConnection);
    }
}
