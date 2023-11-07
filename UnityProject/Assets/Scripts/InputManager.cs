using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [HideInInspector]
    public bool armHomePose, armSleepPose;

    public void OnArmHomePose(InputAction.CallbackContext context)
    {
        Debug.Log("OnArmHomePose called");
        if (context.action.triggered)
        {
            armHomePose = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            armHomePose = false;
        }
    }

    public void OnArmSleepPose(InputAction.CallbackContext context)
    {
        Debug.Log("OnArmSleepPose called");
        if (context.action.triggered)
        {
            armSleepPose = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            armSleepPose = false;
        }
    }
}
