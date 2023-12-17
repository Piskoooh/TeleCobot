using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agora.Rtc;

//Agoraのコールバックに関するスクリプト
internal class UserEventHandler : IRtcEngineEventHandler
{
    private readonly AgoraManager agoraManager;

    internal UserEventHandler(AgoraManager videoSample)
    {
        agoraManager = videoSample;
    }

    // This callback is triggered when the local user joins the channel.
    public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
    {
        int build = 0;
        Debug.Log("Agora: OnJoinChannelSuccess ");
        Debug.Log(string.Format("sdk version: {0}",
            agoraManager.rtcEngine.GetVersion(ref build)));
        Debug.Log(string.Format("sdk build: {0}",
            build));
        Debug.Log(
            string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
                            connection.channelId, connection.localUid, elapsed));
    }
    public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
    {
        Debug.Log("OnRejoinChannelSuccess");
    }
    public override void OnError(int err, string msg)
    {
        Debug.LogError(string.Format("OnError err: {0}, msg: {1}", err, msg));
    }
    public override void OnConnectionStateChanged(RtcConnection connection, CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
    {
        Debug.Log("Connection state changed"
                + "\n New state: " + state
                + "\n Reason: " + reason);
    }
    public override void OnLastmileProbeResult(LastmileProbeResult result)
    {
        agoraManager.rtcEngine.StopLastmileProbeTest();
        Debug.Log("Probe test finished");
        // The result object contains the detailed test results that help you
        // manage call quality, for example, the downlink jitter.
        Debug.Log("Downlink jitter: " + result.downlinkReport.jitter);
    }
    public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
    {
        Debug.Log("OnLeaveChannel");
        var vs = GameObject.FindGameObjectsWithTag("VideoSurface");
        if (vs != null)
        {
            foreach (var go in vs)
            {
                if (go.gameObject.name != "0")
                    AgoraManager.Destroy(go);
            }
        }
    }
    public override void OnRtcStats(RtcConnection connection, RtcStats rtcStats)
    {
        string msg = "";
        msg = rtcStats.userCount + " user(s)";
        msg += "\nPacket loss rate: " + rtcStats.rxPacketLossRate;
        Debug.Log(msg);
    }
    public override void OnAudioDeviceStateChanged(string deviceId, MEDIA_DEVICE_TYPE deviceType, MEDIA_DEVICE_STATE_TYPE deviceState)
    {
        base.OnAudioDeviceStateChanged(deviceId, deviceType, deviceState);
        Debug.Log(string.Format("{0},{1},{2}", deviceId, deviceType, deviceState));
    }
    public override void OnVideoDeviceStateChanged(string deviceId, MEDIA_DEVICE_TYPE deviceType, MEDIA_DEVICE_STATE_TYPE deviceState)
    {
        base.OnVideoDeviceStateChanged(deviceId, deviceType, deviceState);
        Debug.Log(string.Format("{0},{1},{2}", deviceId, deviceType, deviceState));
    }
    public override void OnNetworkQuality(RtcConnection connection, uint remoteUid, int txQuality, int rxQuality)
    {
        // Use downlink network quality to update the network status
        Debug.Log ("TXquality: "+(QUALITY_TYPE)txQuality+ "\nRXquality: "+(QUALITY_TYPE)rxQuality);
    }
    public override void OnUplinkNetworkInfoUpdated(UplinkNetworkInfo info)
    {
        Debug.Log("OnUplinkNetworkInfoUpdated");
    }

    public override void OnDownlinkNetworkInfoUpdated(DownlinkNetworkInfo info)
    {
        Debug.Log("OnDownlinkNetworkInfoUpdated");
    }
    public override void OnLastmileQuality(int quality)
    {
        Debug.Log(quality);
    }
    public override void OnLocalVideoStateChanged(VIDEO_SOURCE_TYPE source, LOCAL_VIDEO_STREAM_STATE state, LOCAL_VIDEO_STREAM_ERROR errorCode)
    {
        base.OnLocalVideoStateChanged(source, state, errorCode);
    }
    public override void OnRemoteVideoStateChanged(RtcConnection connection, uint remoteUid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
    {
        string msg = "Remote video state changed: \n Uid =" + remoteUid
                + " \n NewState =" + state
                + " \n reason =" + reason
                + " \n elapsed =" + elapsed;

        Debug.Log(msg);
    }
    public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
    {
        Debug.Log(string.Format("OnUserJoined uid: {0} elapsed: {1}", uid, elapsed));
        AgoraManager.MakeVideoView(uid, agoraManager.GetSceneIndex(), agoraManager.GetChannelName());
    }

    public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
    {
        Debug.Log(string.Format("OnUserOffLine uid: {0}, reason: {1}", uid,
            (int)reason));
        AgoraManager.DestroyVideoView(uid);
    }
    public override void OnUserEnableLocalVideo(RtcConnection connection, uint remoteUid, bool enabled)
    {
        base.OnUserEnableLocalVideo(connection, remoteUid, enabled);
        if (enabled)
        {
            AgoraManager.MakeVideoView(remoteUid, agoraManager.GetSceneIndex(), agoraManager.GetChannelName());
        }
        else
        {
            AgoraManager.DestroyVideoView(remoteUid);
        }
    }
    public override void OnRemoteVideoStats(RtcConnection connection, RemoteVideoStats stats)
    {
        string msg = "Remote Video Stats: "
                + "\n User id =" + stats.uid
                + "\n Received bitrate =" + stats.receivedBitrate
                + "\n Total frozen time =" + stats.totalFrozenTime;
        Debug.Log(msg);
    }
    public override void OnTokenPrivilegeWillExpire(RtcConnection connection, string token)
    {
        base.OnTokenPrivilegeWillExpire(connection, token);
    }
    public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions clientRoleOptions)
    {
        Debug.Log("OnClientRoleChanged");
    }
}
