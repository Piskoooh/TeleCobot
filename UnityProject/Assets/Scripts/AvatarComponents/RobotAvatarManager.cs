using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


[RequireComponent(typeof(AvatarSetting))]
public class RobotAvatarManager : MonoBehaviourPun
{
    AvatarSetting avatarSetting;

    private void Awake()
    {
        avatarSetting = GetComponent<AvatarSetting>();
    }

    public void CallAARVP(string vI)
    {
        photonView.RPC(nameof(AdjustArmRangeVisualizerPun), RpcTarget.AllViaServer,vI);
    }

    public void CallEEA(string eE)
    {
        photonView.RPC(nameof(EeAttacherPun), RpcTarget.AllViaServer, eE);
    }

    [PunRPC]
    private void AdjustArmRangeVisualizerPun(string vI, PhotonMessageInfo info)
    {
        if (!info.Sender.IsLocal)
        {
            avatarSetting.sceneMaster.uIMng.visualIndicator= GameObject.FindGameObjectWithTag(vI);
        }
        avatarSetting.sceneMaster.uIMng.visualIndicator.transform.parent = avatarSetting.sceneMaster.rosConnector.arm_base_link.transform;
        avatarSetting.sceneMaster.uIMng.visualIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
        avatarSetting.sceneMaster.uIMng.visualIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    [PunRPC]
    private void EeAttacherPun(string eE, PhotonMessageInfo info)
    {
        if(!info.Sender.IsLocal)
            avatarSetting.sceneMaster.uIMng.eeGripper = GameObject.FindGameObjectWithTag(eE);
    }
}
