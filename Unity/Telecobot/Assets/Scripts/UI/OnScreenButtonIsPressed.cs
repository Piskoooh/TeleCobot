using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(EventTrigger))]
public class OnScreenButtonIsPressed : MonoBehaviour
{
    private bool buttonDownFlag=false;
    private bool prevButtonDownFlag=false;
    [HideInInspector]
    public SceneMaster sceneMaster;

    private void Awake()
    {
        sceneMaster = GameObject.Find("SceneMaster").GetComponent<SceneMaster>();
    }
    // Update is called once per frame
    void Update()
    {
        if(buttonDownFlag)
        {
            if(this.gameObject.name=="SleepPoseButton")
                sceneMaster.inputMng.OnArmSleepPoseVR(true);
            else if(this.gameObject.name=="HomePoseButton")
                sceneMaster.inputMng.OnArmHomePoseVR(true);
            else if(this.gameObject.name=="MoveArmButton")
                sceneMaster.inputMng.OnMoveArmVR(true);
            else if(this.gameObject.name=="MoveBaseButton")
                sceneMaster.inputMng.OnMoveBaseVR(true);
            else if(this.gameObject.name=="RegisterButton")
                sceneMaster.inputMng.OnSetTargetOrGoalVR(true);
        }
        else if(!buttonDownFlag&&prevButtonDownFlag)
        {
            if (this.gameObject.name == "SleepPoseButton")
                sceneMaster.inputMng.OnArmSleepPoseVR(false);
            else if(this.gameObject.name=="HomePoseButton")
                sceneMaster.inputMng.OnArmHomePoseVR(false);
            else if(this.gameObject.name=="MoveArmButton")
                sceneMaster.inputMng.OnMoveArmVR(false);
            else if(this.gameObject.name=="MoveBaseButton")
                sceneMaster.inputMng.OnMoveBaseVR(false);
            else if(this.gameObject.name=="RegisterButton")
                sceneMaster.inputMng.OnSetTargetOrGoalVR(false);
        }

        prevButtonDownFlag = buttonDownFlag;
    }

    public void OnButtonDown()
    {
        buttonDownFlag = true;
        Debug.Log("Button Down");
    }
    public void OnButtonUp()
    {
        buttonDownFlag = false;
        Debug.Log("Button Up");
    }
}
