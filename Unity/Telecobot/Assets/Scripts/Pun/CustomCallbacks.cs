using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(AvatarSetting))]
public class CustomCallbacks : MonoBehaviourPun, IOnPhotonViewPreNetDestroy, IPunInstantiateMagicCallback
{
    SceneMaster sceneMaster;
    private void Awake()
    {
        sceneMaster = GameObject.FindGameObjectWithTag("SceneMaster").GetComponent<SceneMaster>();
    }
    private void OnEnable()
    {
        // PhotonViewのコールバック対象に登録する
        photonView.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        // PhotonViewのコールバック対象の登録を解除する
        photonView.RemoveCallbackTarget(this);
    }

    void IPunInstantiateMagicCallback.OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
    {
        Debug.Log($"{info.Sender.NickName} が {info.photonView.name}({info.photonView.ViewID}) をインスタンス化しました");
        if (info.photonView.GetComponent<AvatarSetting>() != null)
        {
            sceneMaster.photonMng.AddToRoleDic(info.photonView.ViewID, (int)info.photonView.GetComponent<AvatarSetting>().avatarRole);
        }
        if (info.photonView.GetComponent<RobotAvatarSetting>() != null)
        {
            sceneMaster.photonMng.AddToRobotDic(info.photonView.ViewID, (int)info.photonView.GetComponent<RobotAvatarSetting>().robotRosConnection);
            sceneMaster.photonMng.robotList.Add(info.photonView.ViewID);
            sceneMaster.photonMng.robotList.Sort();
        }
    }

    // ネットワークオブジェクトが破棄される直前に呼ばれるコールバック
    void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
    {
        Debug.Log($"{rootView.name}({rootView.ViewID}) が破棄されます");
        if (rootView.GetComponent<AvatarSetting>() != null)
        {
            sceneMaster.photonMng.RemoveFromRoleDic(rootView.ViewID);
        }
        if (rootView.GetComponent<RobotAvatarSetting>() != null)
        {
            sceneMaster.photonMng.RemoveFromRobotDic(rootView.ViewID);
            sceneMaster.photonMng.robotList.Remove(rootView.ViewID);
            sceneMaster.photonMng.robotList.Sort();
        }
    }
}
