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

    public override void OnConnectionStateChanged(RtcConnection connection, CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
    {
        Debug.Log("Connection state changed"
                + "\n New state: " + state
                + "\n Reason: " + reason);
    }

    public override void OnLastmileQuality(int quality)
    {
        Debug.Log(quality);
    }

    public override void OnLastmileProbeResult(LastmileProbeResult result)
    {
        agoraManager.rtcEngine.StopLastmileProbeTest();
        Debug.Log("Probe test finished");
        // The result object contains the detailed test results that help you
        // manage call quality, for example, the downlink jitter.
        Debug.Log("Downlink jitter: " + result.downlinkReport.jitter);
    }

    public override void OnNetworkQuality(RtcConnection connection, uint remoteUid, int txQuality, int rxQuality)
    {
        // Use downlink network quality to update the network status
        Debug.Log (rxQuality);
    }

    public override void OnRtcStats(RtcConnection connection, RtcStats rtcStats)
    {
        string msg = "";
        msg = rtcStats.userCount + " user(s)";
        msg = "Packet loss rate: " + rtcStats.rxPacketLossRate;
        Debug.Log(msg);
    }

    public override void OnRemoteVideoStateChanged(RtcConnection connection, uint remoteUid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
    {
        string msg = "Remote video state changed: \n Uid =" + remoteUid
                + " \n NewState =" + state
                + " \n reason =" + reason
                + " \n elapsed =" + elapsed;

        Debug.Log(msg);
    }
    public override void OnRemoteVideoStats(RtcConnection connection, RemoteVideoStats stats)
    {
        string msg = "Remote Video Stats: "
                + "\n User id =" + stats.uid
                + "\n Received bitrate =" + stats.receivedBitrate
                + "\n Total frozen time =" + stats.totalFrozenTime;
        Debug.Log(msg);
    }

    public override void OnError(int err, string msg)
    {
        Debug.LogError(string.Format($"OnError err: {0}, msg: {1}", err, msg));
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

    public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
    {
        Debug.Log("OnLeaveChannel");
    }

    public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions clientRoleOptions)
    {
        Debug.Log("OnClientRoleChanged");
    }

    public override void OnTokenPrivilegeWillExpire(RtcConnection connection, string token)
    {
        base.OnTokenPrivilegeWillExpire(connection, token);
    }

    public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
    {
        Debug.Log(string.Format("OnUserJoined uid: {0} elapsed: {1}", uid, elapsed));
        // Save the remote user ID in a variable.
    }

    public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
    {
        Debug.Log(string.Format("OnUserOffLine uid: {0}, reason: {1}", uid,
            (int)reason));
    }

    public override void OnUplinkNetworkInfoUpdated(UplinkNetworkInfo info)
    {
        Debug.Log("OnUplinkNetworkInfoUpdated");
    }

    public override void OnDownlinkNetworkInfoUpdated(DownlinkNetworkInfo info)
    {
        Debug.Log("OnDownlinkNetworkInfoUpdated");
    }
    public override void OnVideoDeviceStateChanged(string deviceId, MEDIA_DEVICE_TYPE deviceType, MEDIA_DEVICE_STATE_TYPE deviceState)
    {
        base.OnVideoDeviceStateChanged(deviceId, deviceType, deviceState);
        Debug.Log(string.Format("{0},{1},{2}", deviceId, deviceType, deviceState));
    }
}
