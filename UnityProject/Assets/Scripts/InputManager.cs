using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//InputSystemに入力(Action)があった際に行う処理をまとめたスクリプト
//有効化するActionMapの切り替えもここで制御するかも。
public class InputManager : MonoBehaviour
{
    [HideInInspector]
    public bool speedInc,speedDec,speedCourse,speedFine,gripperOpen, gripperClose,
        armHomePose, armSleepPose, panTiltHome, switchMode;

    [HideInInspector]
    public float gripperPwm, waistRotate, eeZ, eeY, baseX, baseRotate, eeRoll, eePitch, pan, tilt;

    public void OnGripperPwm(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperPwm called");
        if (context.action.triggered)
        {
            gripperPwm = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            gripperPwm = 0;
        }
    }

    public void OnGripperOpen(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperOpen called");
        if (context.action.triggered)
        {
            gripperOpen = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            gripperOpen = false;
        }
    }

    public void OnGripperClose(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperClose called");
        if (context.action.triggered)
        {
            gripperClose = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            gripperClose = false;
        }
    }

    public void OnWaistRotate(InputAction.CallbackContext context)
    {
        Debug.Log("OnWaistRotate called");
        if (context.action.triggered)
        {
            waistRotate = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            waistRotate = 0;
        }
    }

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

    public void OnPanTiltHome(InputAction.CallbackContext context)
    {
        Debug.Log("OnPanTiltHome called");
        if (context.action.triggered)
        {
            panTiltHome = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            panTiltHome = false;
        }
    }

    public void OnSwitchMode(InputAction.CallbackContext context)
    {
        Debug.Log("OnSwitchMode called");
        if (context.action.triggered)
        {
            switchMode = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            switchMode = false;
        }
    }

    public void OnEeZ(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeZ called");
        if (context.action.triggered)
        {
            eeZ = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            eeZ = 0;
        }
    }

    public void OnEeY(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeY called");
        if (context.action.triggered)
        {
            eeY = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            eeY = 0;
        }
    }

    public void OnBaseX(InputAction.CallbackContext context)
    {
        Debug.Log("OnBaseX called");
        if (context.action.triggered)
        {
            baseX = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            baseX = 0;
        }
    }

    public void OnBaseRotate(InputAction.CallbackContext context)
    {
        Debug.Log("OnBaseRotate called");
        if (context.action.triggered)
        {
            baseRotate = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            baseRotate = 0;
        }
    }

    public void OnEeRoll(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeRoll called");
        if (context.action.triggered)
        {
            eeRoll = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            eeRoll = 0;
        }
    }

    public void OnEePitch(InputAction.CallbackContext context)
    {
        Debug.Log("OnEePitch called");
        if (context.action.triggered)
        {
            eePitch = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            eePitch = 0;
        }
    }

    public void OnCameraUpDown(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeCameraUpDown called");
        if (context.action.triggered)
        {
            tilt = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            tilt = 0;
        }
    }
}
