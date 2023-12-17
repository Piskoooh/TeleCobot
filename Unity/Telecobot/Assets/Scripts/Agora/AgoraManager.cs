using Agora_RTC_Plugin.API_Example;
using Agora.Rtc;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

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

    public SceneMaster sceneMaster;
    //接続せれているデバイスを管理
    private IAudioDeviceManager audioDeviceManager;
    private IVideoDeviceManager videoDeviceManager;
    private DeviceInfo[] audioRecordingDeviceInfos;
    private DeviceInfo[] audioPlaybackDeviceInfos;
    private DeviceInfo[] videoDeviceInfos;

    //接続しているデバイスの名前を選択するためのテキスト
    public Toggle micToggle;
    public bool prevMicToggle;
    public Toggle camToggle;
    public bool prevCamToggle;
    public TMP_Dropdown micDevices;
    public TMP_Dropdown speakerDevices;
    public TMP_Dropdown camDevices;
    public TMP_Text statusText;
    [SerializeField]
    private GameObject sphere100Temp;
    static GameObject sphere100;

    static GameObject focusrobot;
    bool isStreaming = false;
    [SerializeField] Button streamingBtn;

    void Awake()
    {
    }
    void Start()
    {
        sphere100 = sphere100Temp;
        micDevices.onValueChanged.AddListener(SetMicDevice);
        speakerDevices.onValueChanged.AddListener(SetSpeakerDevice);
        camDevices.onValueChanged.AddListener(SetCamDevice);
        streamingBtn.onClick.AddListener(OnStreamClick);
        micToggle.isOn = true;
        camToggle.isOn = true;
        prevMicToggle = false;
        prevCamToggle = false;
        isStreaming = false;

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
        //デバイスの使用許可をリクエスト
        PermissionHelper.RequestMicrophonePermission();
        PermissionHelper.RequestCameraPermission();

        //前のフレームでの値から変化した場合に処理を実行
        if(prevCamToggle!=camToggle.isOn)
        {
            //カメラ使用する場合
            if(camToggle.isOn)
            {
                camDevices.interactable = true;
                rtcEngine.EnableLocalVideo(true);
                MakeVideoView(0,sceneMaster.userSettings.CurrentSceneBuildIndex);
                if (!isStreaming)
                {
                    OnStartPreview();
                }
            }
            //カメラ使用しない場合
            else
            {
                camDevices.interactable = false;
                rtcEngine.EnableLocalVideo(false);
                var go = GameObject.Find("0");
                if (!ReferenceEquals(go, null))
                {
                    Destroy(go);
                }
                if (!isStreaming)
                {
                    OnStopPreview();
                }
            }
        }
        //前のフレームでの値から変化した場合に処理を実行
        if(prevMicToggle!=micToggle.isOn)
        {
            //マイク使用する場合
            if(micToggle.isOn)
            {
                micDevices.interactable = true;
                rtcEngine.EnableLocalAudio(true);
            }
            //マイク使用しない場合
            else
            {
                micDevices.interactable = false;
                rtcEngine.EnableLocalAudio(false);
            }
        }

        if (!isStreaming)
        {
            streamingBtn.GetComponentInChildren<TMP_Text>().text = "StartStream";
        }
        else
        {
            streamingBtn.GetComponentInChildren<TMP_Text>().text = "StopStream";
        }

        if(sceneMaster.photonMng.focusRobot != null)
        {
            focusrobot = sceneMaster.photonMng.focusRobot;
        }
        else
        {
            focusrobot = null;
        }

        //次フレームで変化を検出するために現フレームの値を保存
        prevCamToggle = camToggle.isOn;
        prevMicToggle = micToggle.isOn;

        if(statusText!=null)
            statusText.text = "Status: " + rtcEngine.GetConnectionState();
    }
    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        var vs = GameObject.FindGameObjectsWithTag("VideoSurface");
        if (vs != null)
        {
            foreach (var go in vs)
            {
                AgoraManager.Destroy(go);
            }
        }
        if (rtcEngine == null) return;
        rtcEngine.InitEventHandler(null);
        rtcEngine.LeaveChannel();
        rtcEngine.Dispose();
    }

    #region --init method--
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
        //config.dimensions = new VideoDimensions(1920, 960);
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

    internal int GetSceneIndex()
    {
        return sceneMaster.userSettings.CurrentSceneBuildIndex;
    }
    #endregion

    #region --conection method--
    public void OnStreamClick()
    {
        if (isStreaming)
        {
            OnStopStream();
            streamingBtn.GetComponentInChildren<TMP_Text>().text = "StartStream";
            isStreaming = false;
        }
        else
        {
            OnStartStream();
            streamingBtn.GetComponentInChildren<TMP_Text>().text = "StopStream";
            isStreaming = true;
        }
    }

    public void OnStartStream()
    {
        rtcEngine.JoinChannel(_token, _channelName, "", (uint)PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void OnStopStream()
    {
        rtcEngine.LeaveChannel();
    }

    public void OnStartPreview()
    {
        rtcEngine.StartPreview();
        MakeVideoView(0, sceneMaster.userSettings.CurrentSceneBuildIndex);
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
    #endregion

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
        SetCurrentDeviceVolume();
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
        //if (audioDeviceManager != null) audioDeviceManager.SetPlaybackDeviceVolume(100);
    }
    #endregion

    #region --video render ui--

    internal static void MakeVideoView(uint uid,int sceneIndex, string channelId = "")
    {
        var go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            go.GetComponent<VideoSurface>().SetEnable(true);
            return; // reuse
        }
        VideoSurface videoSurface = null;
        // create a GameObject and assign to this new user
        if (uid<100)
        {
            videoSurface = MakeImageSurface(uid.ToString());
        }
        else if (uid>=100)
        {
            videoSurface = MakeSphereSurface(uid.ToString());
        }
        else
        {
            videoSurface = MakePlaneSurface(uid.ToString());
        }
        if (ReferenceEquals(videoSurface, null)) return;
        videoSurface.gameObject.tag = "VideoSurface";

        // configure videoSurface
        if (uid == 0)
        {
            videoSurface.SetForUser(uid, channelId);
        }
        else
        {
            videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        }
        if (uid<100)
        {
            if (sceneIndex != 2)
            {
                videoSurface.OnTextureSizeModify += (int width, int height) =>
                {
                    float scale = (float)height / (float)width;
                    videoSurface.transform.localScale = new Vector3(-5, 5 * scale, 1);
                    Debug.Log("OnTextureSizeModify: " + width + "  " + height);
                };
            }
            else
            {
                videoSurface.OnTextureSizeModify += (int width, int height) =>
                {
                    float scale = (float)height / (float)width;
                    videoSurface.transform.localScale = new Vector3(-1, 1 * scale, 1);
                    Debug.Log("OnTextureSizeModify: " + width + "  " + height);
                };
            }
        }
        videoSurface.SetEnable(true);
    }

    // VIDEO TYPE 1: 3D Object
    private static VideoSurface MakePlaneSurface(string goName)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Plane);

        if (go == null)
        {
            return null;
        }

        go.name = goName;
        var mesh = go.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            Debug.LogWarning("VideoSureface update shader");
            mesh.material = new Material(Shader.Find("Unlit/Texture"));
        }
        // set up transform
        go.transform.Rotate(-90.0f, 0.0f, 0.0f);
        go.transform.position = Vector3.zero;
        go.transform.localScale = new Vector3(0.25f, 0.5f, 0.5f);

        // configure videoSurface
        var videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    // VIDEO TYPE 2: 3D Object
    private static VideoSurface MakeSphereSurface(string goName)
    {
        var go = GameObject.Instantiate<GameObject>(sphere100);

        if (go == null)
        {
            return null;
        }

        go.name = goName;
        var mesh = go.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            Debug.LogWarning("VideoSureface update shader");
            mesh.material = new Material(Shader.Find("Unlit/Texture"));
        }
        // set up transform
        if(focusrobot != null)
        {
            go.transform.parent= focusrobot.transform;
            go.transform.localRotation= Quaternion.Euler(0, 90, 180);
            go.transform.localPosition = Vector3.up;
            go.transform.localScale = Vector3.one * 100;
        }
        else
        {
            go.transform.Rotate(0.0f, 90.0f, 180.0f);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one*100;
        }

        // configure videoSurface
        var videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    // Video TYPE 3: RawImage
    private static VideoSurface MakeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;
        // to be renderered onto
        go.AddComponent<RawImage>();
        // make the object draggable
        go.AddComponent<UIElementDrag>();
        var canvas = GameObject.Find("VideoCanvas");
        if (canvas != null)
        {
            go.transform.parent = canvas.transform;
            Debug.Log("add video view");
        }
        else
        {
            Debug.Log("Canvas is null video view");
        }

        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = new Vector3(2f, 3f, 1f);

        // configure videoSurface
        var videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    internal static void DestroyVideoView(uint uid)
    {
        var go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }
    }
    #endregion
}

#region --Request Permisson--
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
#endregion
