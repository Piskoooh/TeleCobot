using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UIManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public TMP_Text controlMode_Text;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        controlMode_Text.text = "ControlMode: "+playerInput.currentActionMap.name;
    }
}
