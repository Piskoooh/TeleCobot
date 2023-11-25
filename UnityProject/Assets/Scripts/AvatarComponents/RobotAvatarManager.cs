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

    public void CallAARVP()
    {
        photonView.RPC(nameof(AdjustArmRangeVisualizerPun), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void AdjustArmRangeVisualizerPun()
    {
        avatarSetting.sceneMaster.uIMng.visualIndicator.transform.parent = avatarSetting.sceneMaster.rosConnector.arm_base_link.transform;
        avatarSetting.sceneMaster.uIMng.visualIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
        avatarSetting.sceneMaster.uIMng.visualIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
