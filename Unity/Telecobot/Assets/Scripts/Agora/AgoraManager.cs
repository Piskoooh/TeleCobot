using Agora_RTC_Plugin.API_Example;
using Agora.Rtc;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class AgoraManager : MonoBehaviour
{
    [FormerlySerializedAs("appIdInput")]
    [SerializeField] private AppIdInput AppInputConfig;
    [Header("_____________Basic Configuration_____________")]
    [FormerlySerializedAs("APP_ID")]
    [SerializeField]
    private string _appID = "";

    [FormerlySerializedAs("TOKEN")]
    [SerializeField]
    private string _token = "";

    [FormerlySerializedAs("CHANNEL_NAME")]
    [SerializeField]
    private string _channelName = "";
    internal IRtcEngine rtcEngine;

    //接続せれているデバイスを管理
    private IAudioDeviceManager audioDeviceManager;
    private IVideoDeviceManager videoDeviceManager;
    private DeviceInfo[] audioRecordingDeviceInfos;
    private DeviceInfo[] audioPlaybackDeviceInfos;
    private DeviceInfo[] videoDeviceInfos;
    private const int DEVICE_INDEX = 0;

    //接続しているデバイスの名前を選択するためのテキスト
    public TMP_Dropdown micDevices;
    public TMP_Dropdown speakerDevices;
    public TMP_Dropdown camDevices;
    private CameraCapturerConfiguration camConfig;
    private ChannelMediaOptions options;

    bool isPreviewing = false;
    bool isStreaming = false;
    [SerializeField] Button streamingBtn;
    // Start is called before the first frame update

    void Awake()
    {
        micDevices.onValueChanged.AddListener(SetMicDevice);
        speakerDevices.onValueChanged.AddListener(SetSpeakerDevice);
        camDevices.onValueChanged.AddListener(SetCamDevice);
        streamingBtn.onClick.AddListener(OnStreamClick);
    }
    void Start()
    {
        LoadAssetData();
        if (CheckAppId())
        {
            CheckAppId();
            InitRtcEngine();
            SetBasicConfiguration();
#if UNITY_STANDALONE_WIN||UNITY_STANDALONE_OSX
            CallDeviceManagerApi();
#endif
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isStreaming||isPreviewing)
        {
            PermissionHelper.RequestMicrophonePermission();
            PermissionHelper.RequestCameraPermission();
        }
    }
    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (rtcEngine == null) return;
        rtcEngine.InitEventHandler(null);
        rtcEngine.LeaveChannel();
        rtcEngine.Dispose();
    }

    public void OnStreamClick()
    {
        if (isStreaming)
        {
            OnStopStream();
            isStreaming = false;
        }
        else
        {
            OnStartStream();
            isStreaming = true;
        }
    }

    public void OnStartStream()
    {
        rtcEngine.JoinChannel(_token, _channelName);
    }

    public void OnStopStream()
    {
        rtcEngine.LeaveChannel();
    }

    public void OnStartPreview()
    {
        rtcEngine.StartPreview();
    }

    public void OnStopPreview()
    {
        rtcEngine.StopPreview();
    }

    public void StartPublish()
    {
        var options = new ChannelMediaOptions();
        options.publishMicrophoneTrack.SetValue(true);
        options.publishCameraTrack.SetValue(true);
        var nRet = rtcEngine.UpdateChannelMediaOptions(options);
        Debug.Log("UpdateChannelMediaOptions: " + nRet);
    }

    public void StopPublish()
    {
        var options = new ChannelMediaOptions();
        options.publishMicrophoneTrack.SetValue(false);
        options.publishCameraTrack.SetValue(false);
        var nRet = rtcEngine.UpdateChannelMediaOptions(options);
        Debug.Log("UpdateChannelMediaOptions: " + nRet);
    }
    private bool CheckAppId()
    {
        if(_appID.Length > 10)
            return true;
        else
        {
            Debug.Log("Please fill in your appId in API-Example/profile/appIdInput.asset");
            return false;
        }
    }

    //Show data in AgoraBasicProfile
    [ContextMenu("ShowAgoraBasicProfileData")]
    private void LoadAssetData()
    {
        if (AppInputConfig == null) return;
        _appID = AppInputConfig.appID;
        _token = AppInputConfig.token;
        _channelName = AppInputConfig.channelName;
    }

    private void InitRtcEngine()
    {
        rtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngineContext context = new RtcEngineContext(_appID, 0,
                                    CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                                    AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT,AREA_CODE.AREA_CODE_JP);
        rtcEngine.Initialize(context);
        rtcEngine.InitEventHandler(handler);
    }
    private void SetBasicConfiguration()
    {
        rtcEngine.EnableAudio();
        rtcEngine.EnableVideo();
        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions = new VideoDimensions(640, 360);
        config.frameRate = 15;
        config.bitrate = 0;
        rtcEngine.SetVideoEncoderConfiguration(config);
        rtcEngine.SetChannelProfile(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION);
        rtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    }

    internal string GetChannelName()
    {
        return _channelName;
    }

    #region --device Method--
    private void CallDeviceManagerApi()
    {
        GetAudioRecordingDevice();
        GetAudioPlaybackDevice();
        GetVideoDeviceManager();
        SetCurrentDevice();
        //SetCurrentDeviceVolume();
    }

    private void GetAudioRecordingDevice()
    {
        if (micDevices == null || rtcEngine == null) return;
        micDevices.ClearOptions();
        audioDeviceManager = rtcEngine.GetAudioDeviceManager();
        audioRecordingDeviceInfos = audioDeviceManager.EnumerateRecordingDevices();
        Debug.Log(string.Format("AudioRecordingDevice count: {0}", audioRecordingDeviceInfos.Length));
        for (var i = 0; i < audioRecordingDeviceInfos.Length; i++)
        {
            Debug.Log(string.Format("AudioRecordingDevice device index: {0}, name: {1}, id: {2}", i,
                audioRecordingDeviceInfos[i].deviceName, audioRecordingDeviceInfos[i].deviceId));
            micDevices.options.Add(new TMP_Dropdown.OptionData
            {
                text = string.Format("{0},{1}", i,
                audioRecordingDeviceInfos[i].deviceName)
            });
        }
        micDevices.RefreshShownValue();
    }

    private void GetAudioPlaybackDevice()
    {
        if (speakerDevices == null || rtcEngine == null) return;
        speakerDevices.ClearOptions();
        audioDeviceManager = rtcEngine.GetAudioDeviceManager();
        audioPlaybackDeviceInfos = audioDeviceManager.EnumeratePlaybackDevices();
        Debug.Log(string.Format("AudioPlaybackDevice count: {0}", audioPlaybackDeviceInfos.Length));
        for (var i = 0; i < audioPlaybackDeviceInfos.Length; i++)
        {
            Debug.Log($"AudioPlaybackDevice device index: {i}, name: {audioPlaybackDeviceInfos[i].deviceName}," +
                $" id: {audioPlaybackDeviceInfos[i].deviceId}");
            speakerDevices.options.Add(new TMP_Dropdown.OptionData
            {
                text = string.Format("{0},{1}", i,
                               audioPlaybackDeviceInfos[i].deviceName)
            });
        }
        speakerDevices.RefreshShownValue();
    }

    private void GetVideoDeviceManager()
    {
        if (camDevices == null || rtcEngine == null) return;
        camDevices.ClearOptions();
        videoDeviceManager = rtcEngine.GetVideoDeviceManager();
        videoDeviceInfos = videoDeviceManager.EnumerateVideoDevices();
        Debug.Log(string.Format("VideoDeviceManager count: {0}", videoDeviceInfos.Length));
        for (var i = 0; i < videoDeviceInfos.Length; i++)
        {
            Debug.Log(string.Format("VideoDeviceManager device index: {0}, name: {1}, id: {2}", i,
                videoDeviceInfos[i].deviceName, videoDeviceInfos[i].deviceId));
            camDevices.options.Add(new TMP_Dropdown.OptionData
            {
                text = string.Format("{0},{1}", i,
                videoDeviceInfos[i].deviceName)
            });
        }
        camDevices.RefreshShownValue();
    }

    private void SetCurrentDevice()
    {
        SetMicDevice(0);
        SetSpeakerDevice(0);
        SetCamDevice(0);
        //SetCurrentDeviceVolume();
    }
    public void SetMicDevice(int value)
    {
        if (audioDeviceManager != null && audioRecordingDeviceInfos.Length > 0)
        {
            var ret = audioDeviceManager.SetRecordingDevice(audioRecordingDeviceInfos[value].deviceId);
            Debug.Log("SetMicDevice returns: " + ret);
        }
    }
    public void SetSpeakerDevice(int value)
    {
        if (audioDeviceManager != null && audioPlaybackDeviceInfos.Length > 0)
        {
            var ret = audioDeviceManager.SetPlaybackDevice(audioPlaybackDeviceInfos[value].deviceId);
            Debug.Log("SetSpeakerDevice returns: " + ret);
        }
    }
    public void SetCamDevice(int value)
    {
        if (videoDeviceManager != null && videoDeviceInfos.Length > 0)
        {
            var ret = videoDeviceManager.SetDevice(videoDeviceInfos[value].deviceId);
            Debug.Log("SetCamDevice returns: " + ret);
        }
    }
    private void SetCurrentDeviceVolume()
    {
        if (audioDeviceManager != null) audioDeviceManager.SetRecordingDeviceVolume(100);
        if (audioDeviceManager != null) audioDeviceManager.SetPlaybackDeviceVolume(100);
    }
    #endregion
}
public class PermissionHelper
{
    public static void RequestMicrophonePermission()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
	if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
	{                 
		Permission.RequestUserPermission(Permission.Microphone);
	}
#endif
    }

    public static void RequestCameraPermission()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
	if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
	{                 
		Permission.RequestUserPermission(Permission.Camera);
	}
#endif
    }
}
