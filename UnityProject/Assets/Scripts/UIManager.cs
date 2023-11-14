using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public TMP_Text controlMode_Text, punConnection_Text, rosConnection_Text;
    public Button punConnectButton, rosConnectButton;


    // Start is called before the first frame update
    void Start()
    {
        punConnection_Text.text = "Photon : Not Connected";
        rosConnection_Text.text = "Ros : Not Connected";
        punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.interactable = false;

    }

    // Update is called once per frame
    void Update()
    {
        controlMode_Text.text = "ControlMode: "+playerInput.currentActionMap.name;
    }
}
