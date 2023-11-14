using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//InputSystemに入力(Action)があった際に行う処理をまとめたスクリプト
//有効化するActionMapの切り替えもこのスクリプトのStart()とOnSwitch()で制御。
//InputSystemについては https://learning.unity3d.jp/8070/　で学べる。
public class InputManager : MonoBehaviour
{
    [HideInInspector]
    public bool speedInc, speedDec, speedCourse, speedFine, gripperPwmInc, gripperPwmDec, gripperOpen, gripperClose,
        armHomePose, armSleepPose, panTiltHome, rebootError, rebootAll, torqueEnable, torqueDisable,
        moveBase, cartesian, moveit;

    [HideInInspector]
    public float waistRotate, eeX, eeZ, baseX, baseRotate, eeRoll, eePitch, pan, tilt;

    public ControlMode controlMode=ControlMode.ManualControl;
    public PlayerInput playerInput;
    InputActionMap manual;
    InputActionMap semiAuto;

    private void Start()
    {
        manual = playerInput.actions.FindActionMap("LocobotManual");
        semiAuto = playerInput.actions.FindActionMap("LocobotSemiAuto");
        var currentActionMap = playerInput.currentActionMap;

        if (currentActionMap == manual)
        {
            controlMode = ControlMode.ManualControl;
        }
        else if (currentActionMap == semiAuto)
        {
            controlMode = ControlMode.SemiAutomaticControl;
        }
        else controlMode = ControlMode.Unkown;
    }

    public void OnGripperPwmInc(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperPwmInc called");
        if (context.started)
        {
            gripperPwmInc = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            gripperPwmInc = false;
        }
    }

    public void OnGripperPwmDec(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperPwmDec called");
        if (context.started)
        {
            gripperPwmDec = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            gripperPwmDec = false;
        }
    }

    public void OnGripperOpen(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperOpen called");
        if (context.started)
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
        if (context.started)
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
        if (context.started)
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
        if (context.started)
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
        if (context.started)
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
        if (context.started)
        {
            panTiltHome = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            panTiltHome = false;
        }
    }

    public void OnEeZ(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeZ called");
        if (context.performed)
        {
            eeZ = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            eeZ = 0;
        }
    }

    public void OnEeX(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeX called");
        if (context.performed)
        {
            eeX = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            eeX = 0;
        }
    }

    public void OnBaseX(InputAction.CallbackContext context)
    {
        Debug.Log("OnBaseX called");
        if (context.performed)
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
        if (context.performed)
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
        if (context.performed)
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
        if (context.performed)
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
        if (context.performed)
        {
            tilt = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            tilt = 0;
        }
    }

    public void OnRebootError(InputAction.CallbackContext context)
    {
        Debug.Log("OnRebootError called");
        if (context.started)
        {
            rebootError= context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            rebootError = false;
        }
    }

    public void OnRebootAll(InputAction.CallbackContext context)
    {
        Debug.Log("OnRebootAll called");
        if (context.started)
        {
            rebootAll = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            rebootAll = false;
        }
    }

    public void OnTorqueEnable(InputAction.CallbackContext context)
    {
        Debug.Log("OnTorqueEnable called");
        if (context.started)
        {
            torqueEnable = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            torqueEnable = false;
        }
    }

    public void OnTorqueDisable(InputAction.CallbackContext context)
    {
        Debug.Log("OnTorqueDisable called");
        if (context.started)
        {
            torqueDisable = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            torqueDisable = false;
        }
    }


    //ActionMapの切り替え
    public void OnSwitchMode(InputAction.CallbackContext context)
    {
        Debug.Log("OnSwitchMode called");
        if (context.started)
        {
            if (controlMode == ControlMode.ManualControl && playerInput.currentActionMap == manual)
            {
                controlMode = ControlMode.SemiAutomaticControl;
                playerInput.SwitchCurrentActionMap("LocobotSemiAuto");
            }
            else if (controlMode == ControlMode.SemiAutomaticControl && playerInput.currentActionMap == semiAuto)
            {
                controlMode = ControlMode.ManualControl;
                playerInput.SwitchCurrentActionMap("LocobotManual");
            }
        }
    }

    public void OnMoveBase(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveBase called");
    }

    public void OnMoveArm(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveArm called");
    }

    public void OnPubGoal(InputAction.CallbackContext context)
    {
        Debug.Log("OnSetGoal called");
    }

    public void OnMoveTargetX(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveTargetX called");
    }

    public void OnMoveTargetY(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveTargetY called");
    }

    public void OnMoveTargetZ(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveTargetZ called");
    }
}
