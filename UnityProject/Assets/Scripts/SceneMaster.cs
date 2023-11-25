using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
