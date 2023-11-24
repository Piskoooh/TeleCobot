using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMaster : MonoBehaviour
{
    public UserSettings userSettings;
    public PhotonManager photonMng;
    public RosConnector rosConnector;
    public InputManager inputMng;
    public UIManager uIMng;

    private void Awake()
    {
        userSettings = GameObject.Find("UserSettings").GetComponent<UserSettings>();
    }
}
