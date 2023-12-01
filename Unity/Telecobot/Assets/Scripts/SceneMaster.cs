using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

[RequireComponent(typeof(PhotonManager))]
[RequireComponent(typeof(RosConnector))]
public class SceneMaster : MonoBehaviour
{
    public UserSettings userSettings;
    public PhotonManager photonMng;
    public RosConnector rosConnector;
    public InputManager inputMng;
    public UIManager uIMng;

    private void Awake()
    {
        userSettings = GameObject.FindGameObjectWithTag("UserSettings").GetComponent<UserSettings>();
        if(userSettings.userType == UserType.Remote_VR)
        {
            var manualXRControl = new ManualXRControl();
            StartCoroutine(manualXRControl.StartXRCoroutine());
        }
    }

    private void OnApplicationQuit()
    {
        if (XRGeneralSettings.Instance && XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            var manualXRControl = new ManualXRControl();
            manualXRControl.StopXR();
        }
    }
}

public class ManualXRControl
{
    public IEnumerator StartXRCoroutine()
    {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
    }

    public void StopXR()
    {
        Debug.Log("Stopping XR...");

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR stopped completely.");
    }
}
